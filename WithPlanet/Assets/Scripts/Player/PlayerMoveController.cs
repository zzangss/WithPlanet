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
    public bool isFlipped = false; //이동방향이 오른쪽이면 뒤집는다.

    [Tooltip("걷는 소리 오디오 클립")]
    [SerializeField] private AudioClip[] footstepSounds;


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

       
    }

    void FixedUpdate()
    {
        rigid.velocity = moveDirection;
    }


    void LateUpdate()
    {
        anim.SetFloat("Speed", moveDirection.magnitude);

        //플레이어의 이동 방향에 따라 스프라이트를 뒤집기
        if (h != 0)
        {
            spriter.flipX = h > 0; // 
            isFlipped = h > 0; // -> : true / <- : false
        }
    }

    //audio 출력
    public void PlayFootstepSound()
    {
        if (footstepSounds != null && footstepSounds.Length > 0)
        {
            // 1. 배열의 인덱스 범위 안에서 무작위 숫자를 하나 고릅니다. (예: 0, 1, 2, 3 중 하나)
            int randomIndex = Random.Range(0, footstepSounds.Length);

            // 2. 무작위로 고른 오디오 클립을 AudioManager에 전달해 재생합니다.
            AudioManager.Instance.PlaySfx(footstepSounds[randomIndex]);
        }
           
    }

    /*private bool CheckGrounded(out RaycastHit hit)
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
    }*/

}

