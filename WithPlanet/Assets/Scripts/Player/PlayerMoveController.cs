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
    [SerializeField] private float maxSlopeAngle = 40f; // 해당 각도보다 가파르면 이동 불가 


    [Header("중력/접지 스냅")]
    [SerializeField] private float extraGravity = 20f;           // 공중 가속-추가중력
    [SerializeField] private float groundStickForce = 25f;       // 지면일 때 바닥으로 눌러주는 가속
    [SerializeField] private float snapProbeDistance = 0.9f;     // 막 떠났을 때 아래로 탐침 거리
    [SerializeField] private float coyoteTime = 0.1f;           // 막 떨어진 뒤 스냅 허용 시간

    private bool isGrounded;
    private Vector3 groundNormal = Vector3.up;
    private float lastGroundedTime;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();

        rigid.constraints = RigidbodyConstraints.FreezeRotation;
        // 빠른이동 시 충돌 감지 
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rigid.interpolation = RigidbodyInterpolation.Interpolate;
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
        if (PauseController.isPaused)
        {
            rigid.velocity = Vector3.zero;
            return;
        }

        // 회전속도 초기화(물리로 도는 것 방지)
        rigid.angularVelocity = Vector3.zero;

        // 1) 접지 체크 + 노멀
        bool groundedNow = CheckGrounded(out RaycastHit groundHit);

        if (groundedNow) lastGroundedTime = Time.time;

        // 2) 목표 속도: 경사 접선에 투영
        Vector3 desiredVelocity = ProjectOnGround(moveDirection, groundNormal);

        // 2-1) 너무 가파른 경사면이면 수평 성분만 남기기
        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);
        if (slopeAngle > maxSlopeAngle)
        {
            desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, Vector3.up); // 사실상 경사 무시
        }

        // 현재 속도에 x/z만 덮어쓰기
        Vector3 v3 = rigid.velocity;
        v3.x = desiredVelocity.x;
        v3.z = desiredVelocity.z;

        // 3) 접지 스냅 + 추가 중력
        if (groundedNow)
        {
            // 지면 위라면: 하강 성분은 0 또는 하강만 유지
            v3.y = Mathf.Min(v3.y, 0f);

            // 바닥으로 살짝 눌러 접지 유지(가속)
            rigid.AddForce(-groundNormal * groundStickForce, ForceMode.Acceleration);
        }
        else
        {
            // 방금까지는 붙어있었는데 한 프레임 떠버린 경우: 아래로 SphereCast 하여 붙이기
            if (Time.time - lastGroundedTime <= coyoteTime && v3.y <= 0f)
            {
                Vector3 origin = transform.position + Vector3.up * groundCheckOffset;
                if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit snapHit, snapProbeDistance, groundMask, QueryTriggerInteraction.Ignore))
                {
                    float a = Vector3.Angle(snapHit.normal, Vector3.up);
                    if (a <= maxSlopeAngle)
                    {
                        // 살짝 내리꽂아 붙이기
                        transform.position = snapHit.point + snapHit.normal * groundCheckRadius;
                        groundNormal = snapHit.normal;
                        groundedNow = true;
                        // 속도는 경사 접선으로 정리
                        v3 = ProjectOnGround(v3, groundNormal);
                        v3.y = Mathf.Min(v3.y, 0f);
                    }
                }
            }

            // 공중일 때는 추가 중력(가속)으로 빨리 내려오게
            rigid.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }

        rigid.velocity = v3;

        // 최종 플래그 저장
        isGrounded = groundedNow;
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

    private bool CheckGrounded(out RaycastHit hit)
    {
        Vector3 origin = transform.position + Vector3.up * groundCheckOffset;

        if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down,
                               out hit, groundCheckOffset + 0.55f,
                               groundMask, QueryTriggerInteraction.Ignore))
        {
            groundNormal = hit.normal;
            float angle = Vector3.Angle(groundNormal, Vector3.up);
            return angle <= maxSlopeAngle + 5f; // 판정은 약간 관대하게(+5° 버퍼)
        }

        groundNormal = Vector3.up;
        return false;
    }

    private static Vector3 ProjectOnGround(Vector3 v, Vector3 groundN)
    {
        return Vector3.ProjectOnPlane(v, groundN);
    }

}

