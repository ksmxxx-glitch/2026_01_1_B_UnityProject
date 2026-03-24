using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;

    public Rigidbody rb;

    public bool isGrounded = true;

    public int coinCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //움직임 입력
        float moveHorizontal = Input.GetAxis("Horizontal");          //수평 이동
        float moveVertical = Input.GetAxis("Vertical");             //수직 이동

        //속도 값으로 직접 이동
        rb.linearVelocity = new Vector3 (moveHorizontal * moveSpeed, rb.linearVelocity.y, moveVertical * moveSpeed);

        //점프 입력
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
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

}
