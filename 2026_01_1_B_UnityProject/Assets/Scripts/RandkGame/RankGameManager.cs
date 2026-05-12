using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class RankGameManager : MonoBehaviour
{
    public int gridWidth = 7;
    public int gridHeight = 7;
    public float CellSize = 1.3f;
    public GameObject cellPrefab;
    public Transform gridContainer;

    public GameObject rankPrefabs;
    public Sprite[] rankSpriteds;
    public int maxRankLevel = 7;

    public GridCell[,] grid;

    void initializeGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(
                    x * CellSize - (gridWidth * CellSize / 2) + CellSize / 2,
                    y * CellSize - (gridHeight * CellSize / 2) + CellSize / 2,
                    1f
                );
                
                GameObject cellObj = Instantiate(cellPrefab, position, Quaternion.identity, gridContainer);
                GridCell cell = cellObj.AddComponent<GridCell>();
                cell.Initialize(x, y);

                grid[x, y] = cell;  // 배열에 저장
            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initializeGrid();

        for (int i = 0; i < 4; i++)
        {
            SpawnNewRank();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnNewRank();
        }
    }

    public DraggableRank CreateRankInCell(GridCell cell, int level)
    {
        if (cell == null || !cell.isEmpty()) return null; //비어있는 칸이 아니면 생성 실패

        level = Mathf.Clamp(level, 1, maxRankLevel); //레벨 범위 확인

        Vector3 rankPosition = new Vector3(cell.transform.position.x, cell.transform.position.y, 0f); //계급장 위치 설정

        //드래그 가능한 계급장 컴포넌트를 추가

        GameObject rankObj = Instantiate(rankPrefabs, rankPosition, Quaternion.identity, gridContainer);
        rankObj.name = "Rank_Level_" + level;

        DraggableRank rank = rankObj.AddComponent<DraggableRank>();

        rank.SetRankLevel(level);

        cell.SetRank(rank);

        return rank;
    }

    private GridCell FineEmptyCell() //비어 있는 칸 찾기
    {
        List<GridCell> emptyCells = new List<GridCell>(); //빈 칸들을 저장할 리스트

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].isEmpty()) //칸이 비어 있다면 리스트에 추가
                {
                    emptyCells.Add(grid[x, y]);
                }
            }
        }

        if (emptyCells.Count == 0) //빈 칸이 없으면 null 값 반환
        {
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)]; //랜덤 하게 빈칸 하나 선택
    }

    public bool SpawnNewRank() //새 계급장 생성
    {
        GridCell emptyCell = FineEmptyCell(); //1. 빈칸 찾기
        if (emptyCell == null) return false; //2. 빈칸 없으면 실패

        int rankLevel = Random.Range(0, 100) < 80 ? 1 : 2; //80% 확률로 레벨 1, 20% 확률로 2

        CreateRankInCell(emptyCell, rankLevel); //3. 계급장 생성 및 설정

        return true;
    }

    public GridCell FindClosesteCell(Vector3 position) //가장 가까운 칸 찾기
    {
        for (int x = 0; x < gridWidth; x++) //1. 먼저 위치가 포함된 칸 확인
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].ContainsPosition (position))
                {
                    return grid[x, y];
                }
            }
        }

        GridCell clossestCell = null; //2. 없다면 가장 가까운 칸 찾기
        float closestDistance = float.MaxValue;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float distance = Vector3.Distance(position, grid[x, y].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    clossestCell = grid[x, y];
                }
            }
        }

        if (closestDistance > CellSize * 2) //3. 너무 멀면 null 반환
        {
            return null;
        }
        return clossestCell;
    }

    public void RemoveRank(DraggableRank rank) //계급장 제거
    {
        if (rank == null) return; //계급장이 칸에 없다면 제거 실패
    
        if (rank.currentCell != null)
        {
            rank.currentCell.currentRank = null; //칸에서 계급장 제거
        }
        Destroy(rank.gameObject); //게임 오브젝트 제거

    }

    public void MergeRanks(DraggableRank draggableRank, DraggableRank targetRank)
    {
        if (draggableRank == null || targetRank == null || draggableRank.rankLevel != targetRank.rankLevel)
        {
            //같은 레벨이 아니면 머지가 되지 않는다.
            if (draggableRank != null) draggableRank.ReturnToOriginalPosition();
            return;
        }

        int newLevel = targetRank.rankLevel + 1;
        if (newLevel > maxRankLevel)
        {
            //최대 레벨을 초과할 경우의 처리 (현재는 드래그한 것만 제거하는 로직)
            RemoveRank(draggableRank);
            return;
        }

        targetRank.SetRankLevel(newLevel); //타겟 계급장의 레벨을 올림
        RemoveRank(draggableRank);         //드래그했던 계급장은 제거
    }

}
