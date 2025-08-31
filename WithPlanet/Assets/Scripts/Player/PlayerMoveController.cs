using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody rigid;
    private Animator anim;
    private SpriteRenderer spriter;

    // 입력값 캐시 
    private float h;
    private float v;
    private Vector3 moveDirection;

    [Header("이동속도")]
    public float normalSpeed = 15f;
    public float runSpeed = 30f;
    public float currentSpeed = 15f;
    public bool isFlipped = false;

    private LayerMask groundMask = ~0;
    private float groundCheckRadius = 0.28f;
    private float groundCheckOffset = 0.2f;
    private float extraGravity = 8f; // 슬립 방지용 중력
    private float maxSlopeAngle = 75f; // 해당 각도보다 가파르면 이동 불가 

    private bool isGrounded;
    private Vector3 groundNormal = Vector3.up;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();

        rigid.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (PauseController.isPaused)
        {
            // 일시정지 상태에서는 이동을 하지 않음
            rigid.velocity = Vector3.zero; // 속도를 0으로 설정하여 이동을 멈춤
            anim.SetFloat("Speed", 0f); // 애니메이션 속도도 0으로 설정
            moveDirection = Vector3.zero; // 이동 방향도 초기화

            return;
        }
        // 이동 입력 처리
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : normalSpeed;

        Vector3 worldDirection = new Vector3(h, 0f, v);
        moveDirection = worldDirection.normalized * currentSpeed;

        anim.SetFloat("Speed", new Vector2(h, v).magnitude);
    }

    void FixedUpdate()
    {
        // 게임 일시정지 
        if (PauseController.isPaused)
        {
            rigid.velocity = Vector3.zero;
            return;
        }

        // 회전 속도 초기화
        rigid.angularVelocity = Vector3.zero; 

        //바닥체크&계산
        Vector3 origin = transform.position + Vector3.up * groundCheckOffset;
        RaycastHit hit;

        if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down,
                          out hit, groundCheckOffset + 0.45f,
                          groundMask, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
            groundNormal = hit.normal;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
        //경사면 투영 이동
        Vector3 desiredVelocity = Vector3.ProjectOnPlane(moveDirection, groundNormal);

        //경사 각도 제한
        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);
        if (slopeAngle > maxSlopeAngle)
        {
            desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, Vector3.up);
        }
        //속도 적용
        Vector3 velocity = rigid.velocity;
        velocity.x = desiredVelocity.x;
        velocity.z = desiredVelocity.z;

        //중력 작용
        if (isGrounded)
        {
            velocity += Vector3.down * extraGravity * Time.fixedDeltaTime;
        }

        rigid.velocity = velocity;
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", moveDirection.magnitude);

        //플레이어의 이동 방향에 따라 스프라이트를 뒤집기
        if (h != 0)
        {
            spriter.flipX = h > 0;
            isFlipped = h > 0;
        }
    }
}

