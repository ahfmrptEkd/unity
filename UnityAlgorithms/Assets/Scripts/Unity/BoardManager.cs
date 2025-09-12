using UnityEngine;
using UnityAlgorithms.Games.ConnectFour;

namespace UnityAlgorithms.Unity
{
    public class BoardManager : MonoBehaviour
    {
        // --- Inspector에서 연결할 변수들 ---
        [Header("Game Objects")]
        public GameObject cellPrefab; // 보드 한 칸으로 사용할 프리팹
        public GameObject redDiscPrefab; // 빨간색 디스크 프리팹
        public GameObject yellowDiscPrefab; // 노란색 디스크 프리팹

        [Header("Board Settings")]
        public float cellSpacing = 1.1f; // 칸 사이의 간격

        // 게임 보드의 크기 (나중에 GameConstants에서 가져올 수 있음)
        private const int Rows = 6;
        private const int Cols = 7;

        // 나중에 GameManager와 통신하기 위한 변수
        private GameManager gameManager;

        void Start()
        {
            // GameManager를 찾아서 연결
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found in the scene!");
            }

            // 초기 보드 생성
            CreateBoard();
        }

        // 3D 보드를 시각적으로 생성하는 함수
        private void CreateBoard()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    // cellPrefab을 사용해 보드판의 각 칸을 생성하는 로직 (나중에 구현)
                }
            }
            Debug.Log("Visual board created.");
        }

        // 게임 상태에 맞게 보드를 다시 그리는 함수 (나중에 구현)
        public void DrawBoard(ConnectFourState state)
        {
            // TODO: 현재 게임 상태에 맞게 디스크들을 배치
        }

        // 디스크를 특정 위치에 생성하는 함수 (나중에 구현)
        public void PlaceDisc(int row, int col, bool isPlayerOne)
        {
            // TODO: 적절한 색상의 디스크 프리팹을 생성해서 배치
        }
    }
}