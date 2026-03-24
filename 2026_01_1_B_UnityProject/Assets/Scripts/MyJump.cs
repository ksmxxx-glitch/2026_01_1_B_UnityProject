using UnityEngine;
using UnityEngine.UI;

public class MyJump : MonoBehaviour
{
    public Rigidbody rigidbody;  //강체 (형태와 크기가 고정된 고체) 물리 현상이 동작 하게 해주는 컴포넌트
    public float power = 20f;      //변수 힘을 선언함
    public float timer = 0;
    public Text TextUI;              //텍스트 UI를 선언함

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
        void Update()
        {
        timer = timer + Time.deltaTime;        //타이머를 상승시킨다..
        TextUI.text = timer.ToString();        //

        if (Input.GetKeyDown(KeyCode.Space)) //스페이스 키를 누르면
            {
                power = power + Random.Range(-100, 200); //power를 랜덤으로 변경시킨다. (-100~200) 사이의 값을 더한다.
                rigidbody.AddForce(transform.up * power); //변수(power)의 위쪽으로 힘을 준다.
            }

            if (this.gameObject.transform.position.y > 5 || this.gameObject.transform.position.y < -3)
            {
                //이 오브젝트의 y 좌표점 위치가 5보다 크거나 -3보다 작으면
                Destroy(this.gameObject); //이 게임 오브젝트를 파괴한다.
            }

        }
    }
    
