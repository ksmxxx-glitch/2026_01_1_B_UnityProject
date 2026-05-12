using UnityEngine;

public class DraggableRank : MonoBehaviour
{
    public int rankLevel = 1;
    public float dragSpeed = 30f;
    public float snapBackSpeed = 20f;

    public bool isDragging = false;
    public Vector3 originalPosition;                  //원래 위치
    public GridCell currentCell;                     //현재 위치한 칸

    public Camera mainCamera;
    public Vector3 dragOffset;
    public SpriteRenderer spriteRenderer;

    public RankGameManager GameManager;

    private void Awake()
    {
        //필요한 컴포넌트 참조 설정
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager = FindAnyObjectByType<RankGameManager>();
    }
        void Start()
        {
            originalPosition = transform.position; //시작 위치 저장
        }

    void Update()
    {
        if (isDragging)
        {
            Vector3 targetPosition = GetMouseWorldPosition() + dragOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime);
        }
        else if (transform.position != originalPosition && currentCell != null) //드래그가 끝났는데 원래 위치로 돌아가야하는 경우
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, snapBackSpeed * Time.deltaTime);
        }
    }

    private void OnMouseDown()
    {
        StartDragging();
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        StopDragging();
    }

    void StartDragging()
    {
        isDragging = true;
        dragOffset = transform.position - GetMouseWorldPosition(); //드래그 시작 시 마우스와의 오프셋 계산
        spriteRenderer.sortingOrder = 0;
    }


    public void MoveToCell(GridCell targetCell)
    {
        if (currentCell != null)
        {
            currentCell.currentRank = null;             //이전 칸에서 계급장 제거
        }
        currentCell = targetCell;                     //새로운 칸으로 업데이트
        targetCell.currentRank = this;                     //새로운 칸에 계급장 배치

        originalPosition = new Vector3(targetCell.transform.position.x, targetCell.transform.position.y, 0);
        transform.position = originalPosition;         //계급장 위치 업데이트
    }

    public void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
    }

    public void MergeWithCell(GridCell targetCell)
    {
        if(targetCell.currentRank == null || targetCell.currentRank.rankLevel != rankLevel)
        {
            ReturnToOriginalPosition();
            return;
        }

        if (currentCell != null)
        {
            currentCell.currentRank = null;             //이전 칸에서 계급장 제거
        }

        // 합치기 실행 MergeRank 함수를 통해서 실행
        GameManager.MergeRanks(this, targetCell.currentRank); //계급장 머지 처리

    }

    public Vector3 GetMouseWorldPosition() //마우스 월드 좌표 구하기
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    public void SetRankLevel(int level)
    {
        rankLevel = level;

        if (GameManager != null && GameManager.rankSprites.Length > level - 1)
        {
            spriteRenderer.sprite = GameManager.rankSprites[level - 1]; //레벨에 맞는 스프라이트로 변경
        }
    }

    void StopDragging() //드래그 종료
    {
        isDragging = false;
        spriteRenderer.sortingOrder = 1;
        GridCell targetCell = GameManager.FindClosesteCell(transform.position); //가장 가까운 칸 찾기

        if (targetCell != null)
        {
            if (targetCell.currentRank == null) //빈칸인 경우 -> 이동
            {
                MoveToCell(targetCell);
            }
            else if (targetCell.currentRank != this && targetCell.currentRank.rankLevel == rankLevel) //같은 랭크일 경우 머지
            {
                MergeWithCell(targetCell);
            }
            else
            {
                ReturnToOriginalPosition(); //유효한 칸이 없으면 기존 위치로 복귀
            }
        }
        else
        {
            ReturnToOriginalPosition(); //유효한 칸이 없으면 기존 위치로 복귀
        }
    }

}

