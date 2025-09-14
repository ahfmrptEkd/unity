using UnityEngine;
using UnityAlgorithms.Algorithms.Core;
using UnityAlgorithms.Games.ConnectFour;
using System.Linq;
using UnityEngine.UI;

// namespace를 UnityAlgorithms 네임스페이스로 묶어주면 좋습니다.
namespace UnityAlgorithms.Unity
{
    public class GameManager : MonoBehaviour
    {
        // --- Inspector에서 연결할 변수들 ---
        [Header("Required Components")]
        public BoardManager boardManager; // BoardManager 스크립트 연결
        public Dropdown difficultyDropdown; // 난이도 선택 드롭다운

        // --- 게임 상태 관련 변수들 ---
        private ConnectFourState currentState; // 현재 게임 상태 (규칙 및 보드 데이터)
        private IAlgorithm aiAlgorithm; // AI 로직

        // 게임이 시작될 때 한번만 호출됩니다.
        void Start()
        {
            // 드롭다운 이벤트 연결
            if (difficultyDropdown != null)
            {
                difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
            }
            
            StartNewGame();
        }

        // 새 게임을 시작하는 함수
        public void StartNewGame()
        {
            // 1. 게임 상태 초기화
            currentState = new ConnectFourState();

            // 2. AI 난이도 설정 (우선 'easy'로 고정)
            aiAlgorithm = AlgorithmFactory.CreateAlgorithm("easy");
            Debug.Log($"AI algorithm '{aiAlgorithm.GetName()}' is ready.");

            // 3. 보드 매니저에게 보드를 그리라고 명령
            // boardManager.DrawBoard(currentState); // (나중에 BoardManager 구현 후 주석 해제)

            Debug.Log("New game started. Player's turn (X).");
        }

        // 컬럼을 클릭했을 때 BoardManager가 호출해 줄 함수
        public void OnColumnSelected(int column)
        {
            // 게임이 끝났거나 AI 턴이면 무시
            if (currentState.IsDone() || !currentState.IsFirst())
            {
                return;
            }

            // 플레이어 턴 처리
            if (currentState.LegalActions().Contains(column))
            {
                // 플레이어 수 실행
                currentState.Progress(column);
                Debug.Log($"Player placed disc in column {column}");

                // 개별 디스크 배치 (중력 적용)
                if (boardManager != null)
                {
                    boardManager.PlaceDiscWithGravity(column, true); // true = 플레이어
                }

                // 게임 종료 체크
                if (currentState.IsDone())
                {
                    HandleGameEnd();
                    return;
                }

                // AI 턴 처리
                ProcessAITurn();
            }
            else
            {
                Debug.Log($"Invalid move: column {column} is full");
            }
        }

        // AI 턴을 처리하는 함수
        private void ProcessAITurn()
        {
            if (currentState.IsDone() || currentState.IsFirst())
            {
                return;
            }

            // AI가 수를 선택
            int aiMove = aiAlgorithm.SelectAction(currentState);
            Debug.Log($"AI selected column: {aiMove}");

            // AI 수 실행
            if (currentState.LegalActions().Contains(aiMove))
            {
                currentState.Progress(aiMove);
                Debug.Log($"AI placed disc in column {aiMove}");

                // 개별 디스크 배치 (중력 적용)
                if (boardManager != null)
                {
                    boardManager.PlaceDiscWithGravity(aiMove, false); // false = AI
                }

                // 게임 종료 체크
                if (currentState.IsDone())
                {
                    HandleGameEnd();
                }
            }
            else
            {
                Debug.LogError($"AI made invalid move: column {aiMove}");
            }
        }

        // 게임 종료를 처리하는 함수
        private void HandleGameEnd()
        {
            var status = currentState.GetWinningStatus();
            if (status == WinningStatus.Win)
            {
                // 현재 턴이 승리 (SwapBoards 후이므로 실제 승자는 반대)
                if (currentState.IsFirst())
                {
                    Debug.Log("Game Over: Player O (Yellow/AI) wins!");
                }
                else
                {
                    Debug.Log("Game Over: Player X (Red) wins!");
                }
            }
            else if (status == WinningStatus.Draw)
            {
                Debug.Log("Game Over: Draw!");
            }

            // 게임 종료 후 추가 처리 (UI 업데이트 등)
            // TODO: 게임 오버 UI 표시, 재시작 버튼 활성화 등
        }

        // 난이도 변경 시 호출되는 함수
        public void OnDifficultyChanged(int index)
        {
            string difficulty = "";
            switch (index)
            {
                case 0: difficulty = "easy"; break;
                case 1: difficulty = "normal"; break;  
                case 2: difficulty = "hard"; break;
                default: difficulty = "easy"; break;
            }

            // AI 알고리즘 변경
            aiAlgorithm = AlgorithmFactory.CreateAlgorithm(difficulty);
            Debug.Log($"Difficulty changed to: {aiAlgorithm.GetName()}");

            // 새 게임 시작 (선택사항)
            // StartNewGame();
        }
    }
}