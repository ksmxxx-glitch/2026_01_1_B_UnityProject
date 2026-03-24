using UnityEngine;

public class MyCharatcter : MonoBehaviour
{
    public int Health = 100;                //체력을 선언한다. (변수 정수 표현.)
    public float Timer = 1.0f;         //타이머를 설정한다.(변수 실수 표현.)

    void Start()
    {
        Health = Health + 100;               //첫 시작 시 100의 체력을 추가한다. 
    }

    // Update is called once per frame
    void Update()
    {
        Timer = Timer - Time.deltaTime;         //시간을 매 프레임마다 감소한다. (Time.deltaTime은 프레임 간의 시간 간격을 나타냄.)         

        if (Timer <= 0)                         //만약 타이머가 0 이하가 되면
        {
            Timer = 1.0f;                     //타이머를 다시 1초로 초기화한다.
            Health = Health - 20;              //체력을 20 감소한다.
        }

        if (Input.GetKeyDown(KeyCode.Space))
            {
            Health = Health + 2;              //스페이스 키를 누르면 체력을 2 증가한다.
        }

        if (Health <= 0)                         //만약 체력이 0 이하가 되면
        {
            Destroy(gameObject);                     //게임 오브젝트를 파괴한다.   
        }

    }

}