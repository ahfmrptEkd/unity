using UnityEngine;
using UnityAlgorithms.Games.ConnectFour;
using System.Collections;

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
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found in the scene!");
            }

            // 초기 보드 생성
            CreateBoard();
        }

        void Update()
        {
            // 마우스 클릭 감지
            if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼
            {
                DetectColumnClick();
            }
        }

        // 마우스 클릭으로 컬럼 감지
        private void DetectColumnClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // BoardCell 태그를 가진 오브젝트인지 확인
                if (hit.collider.gameObject.tag == "BoardCell")
                {
                    // BoardCellInfo에서 컬럼 정보 가져오기
                    var cellInfo = hit.collider.GetComponent<BoardCellInfo>();
                    if (cellInfo != null && gameManager != null)
                    {
                        gameManager.OnColumnSelected(cellInfo.column);
                    }
                }
            }
        }

        // 3D 보드를 시각적으로 생성하는 함수
        private void CreateBoard()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (cellPrefab != null)
                    {
                        // 3D 좌표 계산 (Connect Four는 위에서부터 0번 row)
                        Vector3 position = new Vector3(
                            col * cellSpacing,           // X: 컬럼 위치
                            (Rows - 1 - row) * cellSpacing, // Y: 로우 위치 (뒤집기)
                            0                            // Z: 고정
                        );

                        // cellPrefab 인스턴스 생성
                        GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
                        cell.transform.SetParent(transform); // BoardManager 하위로 설정
                        cell.name = $"Cell_{row}_{col}";

                        // 클릭 감지를 위한 컬럼 태그 설정
                        cell.tag = "BoardCell";
                        
                        // 클릭 감지를 위해 BoxCollider 확인/추가
                        if (cell.GetComponent<Collider>() == null)
                        {
                            cell.AddComponent<BoxCollider>();
                        }

                        // 컬럼 정보를 저장하는 컴포넌트 추가
                        var cellInfo = cell.GetComponent<BoardCellInfo>();
                        if (cellInfo == null)
                        {
                            cellInfo = cell.AddComponent<BoardCellInfo>();
                        }
                        cellInfo.column = col;
                    }
                }
            }
            Debug.Log("Visual board created.");
        }

        // 게임 상태에 맞게 보드를 다시 그리는 함수
        public void DrawBoard(ConnectFourState state)
        {
            // 기존 디스크들 모두 제거
            ClearAllDiscs();

            var myBoard = state.GetMyBoard();
            var enemyBoard = state.GetEnemyBoard();
            bool isFirst = state.IsFirst();

            // 현재 게임 상태에 맞게 디스크들을 배치
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (myBoard[row, col] == 1)
                    {
                        // 현재 턴 플레이어의 말
                        bool isPlayerOne = isFirst;
                        PlaceDiscAt(row, col, isPlayerOne);
                    }
                    else if (enemyBoard[row, col] == 1)
                    {
                        // 상대방의 말
                        bool isPlayerOne = !isFirst;
                        PlaceDiscAt(row, col, isPlayerOne);
                    }
                }
            }
        }

        // 모든 디스크를 제거하는 함수
        private void ClearAllDiscs()
        {
            // "Disc_"로 시작하는 모든 자식 오브젝트 찾기
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child != transform && child.name.StartsWith("Disc_"))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // 중력을 적용하여 디스크를 배치하는 함수 (Connect Four 규칙)
        public void PlaceDiscWithGravity(int column, bool isPlayerOne)
        {
            // 해당 컬럼에서 가장 아래쪽 빈 자리 찾기 (Unity 좌표계 기준)
            int targetRow = -1;
            for (int row = Rows - 1; row >= 0; row--) // 아래부터 위로 검색 (row 5부터 0까지)
            {
                if (IsSlotEmpty(row, column))
                {
                    targetRow = row;
                    break;
                }
            }

            if (targetRow == -1)
            {
                Debug.LogWarning($"Column {column} is full!");
                return;
            }

            // 해당 위치에 디스크 배치
            PlaceDiscAt(targetRow, column, isPlayerOne);
        }

        // 특정 위치가 비어있는지 확인
        private bool IsSlotEmpty(int row, int column)
        {
            // 이미 배치된 디스크가 있는지 확인
            string discName = $"Disc_Red_{row}_{column}";
            string discName2 = $"Disc_Yellow_{row}_{column}";
            
            Transform redDisc = transform.Find(discName);
            Transform yellowDisc = transform.Find(discName2);
            
            return redDisc == null && yellowDisc == null;
        }

        // 디스크를 특정 위치에 생성하는 함수
        private void PlaceDiscAt(int row, int col, bool isPlayerOne)
        {
            // 적절한 프리팹 선택
            GameObject discPrefab = isPlayerOne ? redDiscPrefab : yellowDiscPrefab;
            
            if (discPrefab != null)
            {
                // 3D 좌표 계산 (보드와 동일한 좌표계)
                Vector3 targetPosition = new Vector3(
                    col * cellSpacing,           // X: 컬럼 위치
                    (Rows - 1 - row) * cellSpacing, // Y: 로우 위치 (뒤집기)
                    -0.5f                        // Z: 보드보다 앞쪽에 배치
                );

                // 디스크 생성 (위에서 떨어뜨리기 위해 높은 위치에서 시작)
                Vector3 startPosition = new Vector3(
                    targetPosition.x,
                    targetPosition.y + 3f,  // 3 유닛 높이에서 시작
                    targetPosition.z
                );

                GameObject disc = Instantiate(discPrefab, startPosition, Quaternion.identity);
                disc.transform.SetParent(transform);
                disc.name = $"Disc_{(isPlayerOne ? "Red" : "Yellow")}_{row}_{col}";

                // Rigidbody 추가 (물리 적용을 위해)
                Rigidbody rb = disc.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = disc.AddComponent<Rigidbody>();
                }

                // 물리 설정
                rb.mass = 1f;
                rb.drag = 0.5f;
                rb.freezeRotation = true; // 회전 방지

                // Collider 확인/추가
                if (disc.GetComponent<Collider>() == null)
                {
                    disc.AddComponent<SphereCollider>();
                }

                // 목표 위치로 이동하는 코루틴 시작 (부드러운 낙하)
                StartCoroutine(DropDisc(disc, targetPosition));
            }
            else
            {
                Debug.LogWarning($"Missing disc prefab for player {(isPlayerOne ? "One" : "Two")}");
            }
        }

        // 디스크를 부드럽게 떨어뜨리는 코루틴
        private System.Collections.IEnumerator DropDisc(GameObject disc, Vector3 targetPosition)
        {
            if (disc == null) yield break; // null 체크 추가
            
            float dropTime = 0.8f; // 낙하 시간
            float elapsedTime = 0f;
            Vector3 startPosition = disc.transform.position;

            while (elapsedTime < dropTime && disc != null) // null 체크 추가
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / dropTime;
                
                // Ease-out 애니메이션 (점점 느려짐)
                progress = 1f - (1f - progress) * (1f - progress);
                
                disc.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                yield return null;
            }

            // 최종 위치 확정 (null 체크)
            if (disc != null)
            {
                disc.transform.position = targetPosition;

                // Rigidbody 제거 (더 이상 물리 불필요)
                Rigidbody rb = disc.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Destroy(rb);
                }
            }
        }
    }

    // 보드 셀의 컬럼 정보를 저장하는 컴포넌트
    public class BoardCellInfo : MonoBehaviour
    {
        public int column;
    }
}