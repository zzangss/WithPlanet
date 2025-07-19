using UnityEngine;

public class Simple3DCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;       // 이동 속도
    public float jumpForce = 7f;       // 점프 힘

    private Rigidbody rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 입력 받기
        float moveX = Input.GetAxis("Horizontal");  // A, D 또는 ←, →
        float moveZ = Input.GetAxis("Vertical");    // W, S 또는 ↑, ↓

        // 이동 방향 계산 (XZ 평면)
        Vector3 move = new Vector3(moveX, 0f, moveZ) * moveSpeed;

        // 현재 Y 속도 유지하며 이동 속도 세팅
        Vector3 velocity = new Vector3(move.x, rb.velocity.y, move.z);
        rb.velocity = velocity;

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // 바닥 충돌 체크
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
