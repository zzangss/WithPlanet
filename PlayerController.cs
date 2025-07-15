using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigid;
    public SpriteRenderer spriter;
    public float movespeed = 4f;
    public float runmovespeed = 5f;
    private Vector2 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");

        moveDirection.Normalize();

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runmovespeed : movespeed;
        moveDirection *= currentSpeed;
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector2.zero)
            rigid.MovePosition(rigid.position + moveDirection * Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        if (moveDirection.x != 0)
        {
            spriter.flipX = moveDirection.x < 0;
        }
    }
}