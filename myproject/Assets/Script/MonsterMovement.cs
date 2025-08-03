using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MonsterMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f; //이동속도
    public float patrolDistance = 5f; //순찰거리
    private Vector3 startPosition;
    private int direction = 1; //이동방향
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; //시작위치 저장
    }
    // Update is called once per frame
    void Update()
    {
        if (Turn())
        {
            Flip();
        }
        MoveMonster();
    }
    void MoveMonster()
    {
        transform.Translate(Vector3.right * moveSpeed * direction * Time.deltaTime);
    }
    bool Turn()
    {
        float distanceFromStart = transform.position.x - startPosition.x;

        if (direction == 1 && distanceFromStart >= patrolDistance)
        {
            return true; //오른쪽 끝에 도달
        }
        else if (direction == -1 && distanceFromStart <= -patrolDistance)
        {
            return true; //왼쪽 끝에 도달
        }
        return false;
    }
    void Flip()
    {
        direction *= -1; //방향 반전

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = !sr.flipX; //스프라이트 방향 반전
        }
    }
    void OnDrawGizmos()
    {
        Vector3 currentStart = Application.isPlaying ? startPosition : transform.position;
        Vector3 drawPosition = new Vector3(currentStart.x, currentStart.y - 3f, currentStart.z);

        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Vector3 boxSize = new Vector3(patrolDistance * 2, 1f, 1f);
        Gizmos.DrawCube(drawPosition, boxSize);
    }
}