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
            Debug.Log($"AI aлlgorithm '{aiAlgorithm.GetName()}' is ready.");

            // 3. 보드 매니저에게 보드를 그리라고 명령
            // boardManager.DrawBoard(currentState); // (나중에 BoardManager 구현 후 주석 해제)

            Debug.Log("New game started. Player's turn (X).");
        }

        // 컬럼을 클릭했을 때 BoardManager가 호출해 줄 함수
        public void OnColumnSelected(int column)
        {
            // TODO: 플레이어 턴 처리 로직 구현
            Debug.Log($"Player selected column: {column}");

            // TODO: AI 턴 처리 로직 구현
        }
    }
}