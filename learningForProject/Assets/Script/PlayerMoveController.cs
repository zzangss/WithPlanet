using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private Rigidbody rigid;
    private Animator anim;
    private SpriteRenderer spriter;
    private Vector3 moveDirection;

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

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? 15f : 10f;
        moveDirection *= currentSpeed;
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector3.zero)
            rigid.MovePosition(rigid.position + moveDirection * Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", moveDirection.magnitude);

        if (moveDirection.x != 0)
            spriter.flipX = moveDirection.x > 0;
    }
}
