using UnityEngine;
using UnityAlgorithms.Algorithms.Core;
using UnityAlgorithms.Games.ConnectFour;
using System.Linq;
using UnityEngine.UI;
using TMPro;

// namespace를 UnityAlgorithms 네임스페이스로 묶어주면 좋습니다.
namespace UnityAlgorithms.Unity
{
    public class GameManager : MonoBehaviour
    {
        // --- Inspector에서 연결할 변수들 ---
        [Header("Required Components")]
        public BoardManager boardManager; // BoardManager 스크립트 연결
        public TMP_Dropdown difficultyDropdown; // 난이도 선택 드롭다운 (TMP 버전)
        
        [Header("UI Elements")]
        public TMP_Text turnIndicatorText; // "Player Turn" / "AI Turn" 표시
        public GameObject gameOverPanel; // 게임 오버 패널
        public TMP_Text resultText; // "Player Wins!" 등 결과 텍스트
        public Button playAgainButton; // 다시하기 버튼
        public Button quitGameButton; // 게임 종료 버튼 (게임 오버 시)
        
        [Header("Pause Menu")]
        public GameObject pauseMenuPanel; // 일시정지 메뉴 패널
        public Button resumeButton; // 게임 재개 버튼
        public Button quitPauseButton; // 게임 종료 버튼 (일시정지 메뉴에서)

        // --- 게임 상태 관련 변수들 ---
        private ConnectFourState currentState; // 현재 게임 상태 (규칙 및 보드 데이터)
        private IAlgorithm aiAlgorithm; // AI 로직
        private bool isPaused = false; // 게임 일시정지 상태

        // 게임이 시작될 때 한번만 호출됩니다.
        void Start()
        {
            // 드롭다운 이벤트 연결
            if (difficultyDropdown != null)
            {
                difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
            }
            
            // 버튼 이벤트 연결
            if (playAgainButton != null)
            {
                playAgainButton.onClick.AddListener(RestartGame);
            }
            
            if (quitGameButton != null)
            {
                quitGameButton.onClick.AddListener(QuitGame);
            }
            
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ResumeGame);
            }
            
            if (quitPauseButton != null)
            {
                quitPauseButton.onClick.AddListener(QuitGame);
            }
            
            StartNewGame();
        }
        
        void Update()
        {
            // ESC 키로 일시정지 메뉴 토글
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }

        // 새 게임을 시작하는 함수
        public void StartNewGame()
        {
            // 1. 게임 상태 초기화
            currentState = new ConnectFourState();

            // 2. AI 난이도 설정 (드롭다운 값 기준, 없으면 기본값)
            string difficulty = GetCurrentDifficulty();
            aiAlgorithm = AlgorithmFactory.CreateAlgorithm(difficulty);
            Debug.Log($"AI algorithm '{aiAlgorithm.GetName()}' is ready.");

            // 3. 기존 디스크들 모두 제거 (새 게임이므로)
            if (boardManager != null)
            {
                boardManager.ClearAllDiscs();
            }

            // 4. UI 초기화
            UpdateTurnIndicator();
            HideGameOverPanel();
            HidePauseMenu();
            isPaused = false;

            Debug.Log("New game started. Player's turn (X).");
        }

        // 컬럼을 클릭했을 때 BoardManager가 호출해 줄 함수
        public void OnColumnSelected(int column)
        {
            // 게임이 끝났거나 AI 턴이거나 일시정지 상태면 무시
            if (currentState.IsDone() || !currentState.IsFirst() || isPaused)
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

                // 턴 표시 업데이트
                UpdateTurnIndicator();

                // 게임 종료 체크
                if (currentState.IsDone())
                {
                    HandleGameEnd();
                    return;
                }

                // AI 턴 처리 (딜레이 후)
                StartCoroutine(ProcessAITurnWithDelay());
            }
            else
            {
                Debug.Log($"Invalid move: column {column} is full");
            }
        }

        // AI 턴을 딜레이와 함께 처리하는 코루틴
        private System.Collections.IEnumerator ProcessAITurnWithDelay()
        {
            // 1.5초 대기 (AI가 생각하는 시간)
            yield return new WaitForSeconds(1.5f);
            
            // 실제 AI 턴 처리
            ProcessAITurn();
        }

        // AI 턴을 처리하는 함수
        private void ProcessAITurn()
        {
            if (currentState.IsDone() || currentState.IsFirst() || isPaused)
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

                // 턴 표시 업데이트
                UpdateTurnIndicator();

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
            string resultMessage = "";
            
            if (status == WinningStatus.Win)
            {
                // 현재 턴이 승리 (SwapBoards 후이므로 실제 승자는 반대)
                if (currentState.IsFirst())
                {
                    resultMessage = "AI Wins!";
                    Debug.Log("Game Over: Player O (Yellow/AI) wins!");
                }
                else
                {
                    resultMessage = "Player Wins!";
                    Debug.Log("Game Over: Player X (Red) wins!");
                }
            }
            else if (status == WinningStatus.Draw)
            {
                resultMessage = "Draw!";
                Debug.Log("Game Over: Draw!");
            }

            // 게임 오버 UI 표시
            ShowGameOverPanel(resultMessage);
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

        // 현재 드롭다운에서 선택된 난이도 가져오기
        private string GetCurrentDifficulty()
        {
            if (difficultyDropdown == null) return "easy";

            return difficultyDropdown.value switch
            {
                0 => "easy",
                1 => "normal", 
                2 => "hard",
                _ => "easy"
            };
        }

        // 공개 메소드로 재시작 버튼에서 호출 가능
        public void RestartGame()
        {
            StartNewGame();
        }

        // 턴 표시기 업데이트
        private void UpdateTurnIndicator()
        {
            if (turnIndicatorText == null) return;

            if (currentState.IsFirst())
            {
                turnIndicatorText.text = "Player Turn";
                turnIndicatorText.color = UnityEngine.Color.red;
            }
            else
            {
                turnIndicatorText.text = "AI Turn";
                turnIndicatorText.color = UnityEngine.Color.yellow;
            }
        }

        // 게임 오버 패널 표시
        private void ShowGameOverPanel(string message)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            
            if (resultText != null)
            {
                resultText.text = message;
            }
        }

        // 게임 오버 패널 숨기기
        private void HideGameOverPanel()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
        
        // === 일시정지 및 종료 기능 ===
        
        // 게임 일시정지
        public void PauseGame()
        {
            isPaused = true;
            ShowPauseMenu();
            Time.timeScale = 0f; // 게임 시간 정지
        }
        
        // 게임 재개
        public void ResumeGame()
        {
            isPaused = false;
            HidePauseMenu();
            Time.timeScale = 1f; // 게임 시간 복원
        }
        
        // 일시정지 메뉴 표시
        private void ShowPauseMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
        }
        
        // 일시정지 메뉴 숨기기
        private void HidePauseMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }
        
        // 게임 종료
        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            
            // 에디터에서 실행 중일 때
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}