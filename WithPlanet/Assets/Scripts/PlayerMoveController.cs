using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody rigid;
    private Animator anim;
    private SpriteRenderer spriter;
    private Vector3 moveDirection;
    public float currentSpeed = 15f;
    public float runSpeed = 20f; 
    public bool isFlipped = false; 

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.z = Input.GetAxisRaw("Vertical");
        moveDirection.Normalize();

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : 10f;
        moveDirection *= currentSpeed;
    }

    void FixedUpdate()
    {
        rigid.angularVelocity = Vector3.zero; // 회전 속도 초기화

        if (moveDirection != Vector3.zero)
        {
            rigid.velocity = new Vector3(moveDirection.x, rigid.velocity.y, moveDirection.z);
        }
        else
        {
            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
        }
        
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", moveDirection.magnitude);

        if (moveDirection.x != 0)
        {
            spriter.flipX = moveDirection.x > 0;
            isFlipped = moveDirection.x > 0;
        }

    }

   
}
