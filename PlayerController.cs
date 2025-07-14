using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rigid;
    public float movespeed = 5f;
<<<<<<< HEAD
    public float runmovespeed = 10f;
=======
>>>>>>> origin/코더서예림
    private Vector2 moveDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
<<<<<<< HEAD
        rigid = GetComponent<Rigidbody2D>();
=======
        rigid= GetComponent<Rigidbody2D>();
>>>>>>> origin/코더서예림
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");
<<<<<<< HEAD

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runmovespeed : movespeed;
        moveDirection *= currentSpeed;
=======
>>>>>>> origin/코더서예림
    }

    void FixedUpdate()
    {
<<<<<<< HEAD
        rigid.MovePosition(rigid.position + moveDirection * Time.fixedDeltaTime);
=======
        rigid.MovePosition(rigid.position + moveDirection.normalized * movespeed * Time.fixedDeltaTime);
>>>>>>> origin/코더서예림
    }
}
