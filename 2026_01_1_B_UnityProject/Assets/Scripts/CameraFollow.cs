using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target; // 따라갈 대상  
    public Vector3 offset = new Vector3(0, 5, -10);   // 대상과의 위치 차이
    public float smoothSpeed = 0.125f; // 카메라 이동 속도

    private void LateUpdate()
    {
   
        Vector3 desiredPosition = target.position + offset; // 원하는 위치 계산
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // 부드러운 이동 계산
        transform.position = smoothedPosition; // 카메라 위치 업데이트

            transform.LookAt(target.position); // 대상 바라보기
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
