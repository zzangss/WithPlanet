using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBEhaviour
{
    public float movementSpeed = 3.0f;
    Vector2 moovement = new Vector2()
    Rigidbody2D rigidbody2D;
    void Start()
    {
        rigidbody2D = GetComponent<Ridigbody2D>();
    }

    void Update()
    {

    }

    private void FIxedUpdate()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movementSpeed.Normalize();

        rigidbody2D.velocity = movementSpeed * movementSpeed
    }
}
    

