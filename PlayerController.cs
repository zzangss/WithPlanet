using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rigid;
    public float movespeed = 5f;
    public float runmovespeed = 10f;
    private Vector2 moveDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runmovespeed : movespeed;
        moveDirection *= currentSpeed;
    }

    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + moveDirection * Time.fixedDeltaTime);
    }
}
