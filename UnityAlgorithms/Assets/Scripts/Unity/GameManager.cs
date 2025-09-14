using UnityEngine;
using UnityAlgorithms.Algorithms.Core;
using UnityAlgorithms.Games.ConnectFour;

// namespace를 UnityAlgorithms 네임스페이스로 묶어주면 좋습니다.
namespace UnityAlgorithms.Unity
{
    public class GameManager : MonoBehaviour
    {
        // --- Inspector에서 연결할 변수들 ---
        [Header("Required Components")]
        public BoardManager boardManager; // BoardManager 스크립트 연결

        // --- 게임 상태 관련 변수들 ---
        private ConnectFourState currentState; // 현재 게임 상태 (규칙 및 보드 데이터)
        private IAlgorithm aiAlgorithm; // AI 로직

        // 게임이 시작될 때 한번만 호출됩니다.
        void Start()
        {
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
            if (currentState.IsTerminal() || !currentState.IsPlayerOneTurn())
            {
                return;
            }

            // 플레이어 턴 처리
            if (currentState.IsValidMove(column))
            {
                // 플레이어 수 실행
                currentState = currentState.MakeMove(column);
                Debug.Log($"Player placed disc in column {column}");

                // 보드 업데이트
                if (boardManager != null)
                {
                    boardManager.DrawBoard(currentState);
                }

                // 게임 종료 체크
                if (currentState.IsTerminal())
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
            if (currentState.IsTerminal() || currentState.IsPlayerOneTurn())
            {
                return;
            }

            // AI가 수를 선택
            int aiMove = aiAlgorithm.SelectMove(currentState);
            Debug.Log($"AI selected column: {aiMove}");

            // AI 수 실행
            if (currentState.IsValidMove(aiMove))
            {
                currentState = currentState.MakeMove(aiMove);
                Debug.Log($"AI placed disc in column {aiMove}");

                // 보드 업데이트
                if (boardManager != null)
                {
                    boardManager.DrawBoard(currentState);
                }

                // 게임 종료 체크
                if (currentState.IsTerminal())
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
            if (currentState.HasWinner())
            {
                if (currentState.GetWinner() == 1)
                {
                    Debug.Log("Game Over: Player X (Red) wins!");
                }
                else
                {
                    Debug.Log("Game Over: Player O (Yellow/AI) wins!");
                }
            }
            else
            {
                Debug.Log("Game Over: Draw!");
            }

            // 게임 종료 후 추가 처리 (UI 업데이트 등)
            // TODO: 게임 오버 UI 표시, 재시작 버튼 활성화 등
        }
    }
}