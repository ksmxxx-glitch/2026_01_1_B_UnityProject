using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int x, y;
    public DraggableRank currentRank;
    public SpriteRenderer cellRenderers;

    private void Awake()
    {
        cellRenderers = GetComponent<SpriteRenderer>(); // 컴포넌트 참조 가져오기
    }

    public void Initialize(int gridX, int gridY)
    {
        x = gridX;
        y = gridY;
        name = "Cell_" + x + "." + y;
    }

    public bool isEmpty()
    {
        return currentRank == null;
    }



    public bool ContainsPosition(Vector3Int position)
    {
        Bounds bounds = cellRenderers.bounds;  
        return bounds.Contains(position);
    }

    public void SetRank(DraggableRank rank)          //칸에 계급장 설정
    {
        currentRank = rank;                          //현재 계급장 설정

        if (rank != null)
        {
            rank.currentCell = this;                 //계급장에 현재 칸 정보 알려주기
        }

        rank.originalPosition = new Vector3(transform.position.x, transform.position.y, 0);   //z위치를 0으로 설정 (2D)
        rank.transform.position = new Vector3(transform.position.x, transform.position.y, 0); //계급장 현재 칸 위치로 이동 (2D)
    }


}
