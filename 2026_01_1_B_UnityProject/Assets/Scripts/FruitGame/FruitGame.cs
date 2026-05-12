using UnityEngine;

// Unity 스크립트 참조 0개
public class FruitGame : MonoBehaviour
{
    public GameObject[] fruitPrefabs;                 //과일 프리팹 배열 선언
    public float[] fruitSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f }; //과일 크기 선언

    public GameObject currentFruit;   //현재 들고 있는 과일
    public int currentFruitType;      //현재 들고 있는 과일 타입

    public float fruitStartHeight = 6.0f; //과일 시작시 높이 설정
    public float gameWidth = 5.0f;        //게임판 너비
    public bool isGameOver = false;       //게임 상태
    public Camera mainCamera;             //카메라 참조 (마우스 위치 변환에 필요)

    public float fruitTimer;            //전 시간 설정을 위한 타이머

    void Start()
    {
        mainCamera = Camera.main;   //메인 카메라 참조 가져오기
        SpawnNewFruit();            //게임 시작 시 첫 과일 생성
        fruitTimer = -3.0f;
    }

    void Update()
    {
        if (isGameOver) return; //게임 오버면 리턴

        if (fruitTimer >= 0)        //타이머의 시간이 0보다 클 경우
        {
            fruitTimer -= Time.deltaTime;
        }   

        if (fruitTimer < 0 && fruitTimer > -2)      //타이머 시간이 0과 -2 사이에 있을 때 잰 함수를 초루하고 다른 시간대로 보낸다.
        {
            SpawnNewFruit();
            fruitTimer = -3.0f;     //타이머 시간을 -3으로 보낸다.
        }


        if (currentFruit != null) //현재 생성된 과일이 있을 때만 처리
        {
            Vector3 mousePosition = Input.mousePosition; //마우스 위치를 받아온다.
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition); //마우스 2D 위치를 월드 3D 좌표로 변환

            Vector3 newPosition = currentFruit.transform.position; //과일 위치 업데이트
            newPosition.x = worldPosition.x;

            float halfFruitSize = fruitSizes[currentFruitType] / 2f;

            if (newPosition.x < -gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = -gameWidth / 2 + halfFruitSize;
            }

            if (newPosition.x > gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = gameWidth / 2 + halfFruitSize;
            }

            currentFruit.transform.position = newPosition; //과일 좌표 갱신
        }

        if (Input.GetMouseButtonDown(0) && fruitTimer == -3.0f)
        {
            DropFruit();
        }   

    }


    //과일 생성 함수
    void SpawnNewFruit()
    {
        if (!isGameOver) //게임 오버가 아닐 때만 새 과일 생성
        {
            currentFruitType = Random.Range(0, 3); //0~2 사이의 랜덤 과일 타입

            Vector3 mousePosition = Input.mousePosition; //마우스 위치를 받아온다.
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition); //마우스 2D 위치를 월드 3D 좌표로 변환

            Vector3 spawnPosition = new Vector3(worldPosition.x, fruitStartHeight, 0); //X 좌표만 사용 하고 나머지는 설정한 값으로 한다.

            float halfFruitSize = fruitSizes[currentFruitType] / 2f;

            //X 의 위치가 게임 영역을 벗어나지 않도록 제한
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfFruitSize, gameWidth / 2 - halfFruitSize);

            currentFruit = Instantiate(fruitPrefabs[currentFruitType], spawnPosition, Quaternion.identity); //과일 생성
            currentFruit.transform.localScale = new Vector3(fruitSizes[currentFruitType], fruitSizes[currentFruitType], 1); //과일 크기 설정

            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.gravityScale = 0f; //시작 시에는 중력 스케일을 0 으로 해준다.
            }
        }
    }

    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D> ();
        if (rb!=null)
        {
            rb.gravityScale = 1f;       //중력을 원래 값으로 복구 시킨다.
            currentFruit = null;        //현재 들고 있는 과일 해제
            fruitTimer = 1.0f;          //타이머 초기화
        }
    }

    public void MergeFruits(int fruitType, Vector3 position)
    {
        if (fruitType < fruitPrefabs.Length - 1)   // 마지막 과일 타입이 아니라면
        {
            GameObject newFruit = Instantiate(fruitPrefabs[fruitType + 1], position, Quaternion.identity);  //해당 타입 다음 번호를 생성 시킨다.
            newFruit.transform.localScale = new Vector3(fruitSizes[fruitType + 1], fruitSizes[fruitType + 1], 1.0f);

            //점수 추가 로직 등등
        }
    }   


}
