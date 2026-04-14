using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("기본 이동 설정")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float turnSpeed = 10f;

    [Header("점프 개선 설정")]
    public float fallMultiplier = 2.5f;      //낙하 속도 증가 배수
    public float lowJumpMultiplier = 2.0f;  //낮은 점프 배수

    [Header("지면 감지 설정")]
    public float coyoteTime = 0.15f;         //지면 관성 시간
    public float coyoteTimeCounter;             //관성 타이머
    public bool realGrounded = true;          //실제 지면 감지 여부

    [Header("글라이더 설정")]
    public GameObject gliderObject;           //글라이더 오브젝트
    public float gliderFailSpeed = 1.0f;         //글라이더 낙하 속도
    public float gliderMoveSpeed = 7.0f;         //글라이더 이동 속도
    public float gliderMaxTime = 5.0f;              //글라이더 최대 지속 시간
    public float gliderTimeLeft;              //글라이더 남은 시간
    public bool isGliding = false;              //글라이더 활성화 여부





    public Rigidbody rb;

    public bool isGrounded = true;

    public int coinCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coyoteTimeCounter = 0;

        if (gliderObject != null)           // 글라이더 오브젝트 초기화
        {
            gliderObject.SetActive(false);  //글라이더 오브젝트 비활성화
        }

        gliderTimeLeft = gliderMaxTime;  //글라이더 최대 지속 시간으로 초기화

    }

    // Update is called once per frame
    void Update()
    {

        //지면 감지
        UpdateGroundedState();

        //움직임 입력
        float moveHorizontal = Input.GetAxis("Horizontal");          //수평 이동
        float moveVertical = Input.GetAxis("Vertical");             //수직 이동


        //이동 방향 벡터
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);        //이동 방향 감지

        if (movement.magnitude > 0.1f)   //이동 입력이 있을 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);        //이동 방향으로 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }


        //G키로 글라이더 제어 (누르는 동안 활성화)
        if(Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft > 0)
        {
            if (!isGliding)         // 글라이더 활성화
            {
                EnableGlider();
            }
            
            // 글라이더 사용 시간 감소
       gliderTimeLeft -= Time.deltaTime;

            // 글라이더 시간이 다 되면 비활성화
            if (gliderTimeLeft <= 0)
            {
                DisableGlider();
            }

            else if (isGliding) 
            {
                DisableGlider();
            }
            else if(isGliding)
            {
                DisableGlider();  //G키를 떼면 글라이더 비활성하
            }

        }


        if (isGliding)      //글라이딩 움직임 처리
        {
            //글라이더 사용 중 이동

            ApplyGliderMovement(moveHorizontal, moveVertical);
        }
        else
        {
            //기존 움직임 코드들을 else문 안에 넣는다.




            //속도 값으로 직접 이동
            rb.linearVelocity = new Vector3(moveHorizontal * moveSpeed, rb.linearVelocity.y, moveVertical * moveSpeed);


            //착시 점프 높이 구현
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;   //낙하 속도 증가
            }

            else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))           // 상승 중 점프 버튼 떼면 낮게 점프
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;   //낮은 점프 구현
            }
        }


        //점프 입력
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            realGrounded = false;
            coyoteTimeCounter = 0;

        }

        //지면에 있으면 글라이더 시간 회복 및 글라이더 비활성화
        if (isGrounded)
        {
            if(isGliding)
            {
                DisableGlider();
            }

            //지상에 있을 때 시간 회복
            gliderTimeLeft += Time.deltaTime;
        }

    }

    private void OnCollisionEnter(Collision collision)      //충돌 처리 함수
    {

        if (collision.gameObject.tag == "Ground")
        {
            realGrounded = true;
        }
    }

    private void OnCollisionStay(Collision collision)   //지면과의 충돌이 유지되는지 확인
    {

        if (collision.gameObject.tag == "Ground")
        {
            realGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)   //지면에서 떨어졌는지 확인
    {

        if (collision.gameObject.tag == "Ground")
        {
            realGrounded = false;                       //지면에서 떨어졌기 때문에 false
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinCount++;
            Destroy(other.gameObject);
        }
    }

    //지면 상태 업데이트 함수
    void UpdateGroundedState()
    {
        if (realGrounded)               //실제 지면에 있으면 코요테 타임 리셋
        {
            coyoteTimeCounter = coyoteTime;
            isGrounded = true;
        }
        else
        {
            //실제로는 지면에 없지만 코요테 타임 내 있으면 여전히 지면으로 판단.
            if (coyoteTimeCounter > 0)
            {
                coyoteTimeCounter -= Time.deltaTime;  //시간 지속적으로 감소
                isGrounded = true;
            }
            else
            {
                isGrounded = false;  //코요테 타임 끝나면 지면에서 떨어진 것으로 판단 = False
            }
        }


    }

    //글라이더 활성화 함수
    void EnableGlider()
    {
        isGliding = true;

        //글라이더 오브젝트 표시
        if (gliderObject != null)
        {
            gliderObject.SetActive(true);
        }

        //하강 속도 초기화
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -gliderFailSpeed, rb.linearVelocity.z);

    }

    //글라이더 비활성화 함수
    void DisableGlider()
    {
        isGliding = false;
        //글라이더 오브젝트 숨김
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }
   
        //즉시 낙하하도록 중력 적용
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);  //수직 속도 초기화

    }

    //글라이더 이동 적용
    void ApplyGliderMovement(float horizontal , float vertical)
    {
        //글라이더 효과 : 천천히  떨어지고 수평 방향으로 더 빠르게 이동.

        Vector3 gliderVelocity = new Vector3(
                horizontal * gliderMoveSpeed,
                -gliderFailSpeed,
                vertical * gliderMoveSpeed



            );

        rb.linearVelocity = gliderVelocity;

    }


}
