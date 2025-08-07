using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMonsterMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f; //이동속도
    public float patrolDistance = 5f; //순찰거리

    [Header("Random settings")]
    public float minMoveTime = 1f; //최소이동시간
    public float maxMoveTime = 3f; //최대이동시간
    public float minStopTime = 0.5f; //최소정지시간
    public float maxStopTime = 2f; //최대정지시간

    private Vector3 startPosition;
    private int direction = 1; //이동방향
    private bool isMoving = true; //이동상태
    private float actionTimer = 0f; // 행동타이머
    private float currentActionTime; //현재 행동 지속 시간

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; //시작위치 저장
        SetNewAction();
    }

    // Update is called once per frame
    void Update()
    {
        // 타이머 업데이트
        actionTimer += Time.deltaTime;

        //행동이 끝났다면 새로운 행동 설정
        if (actionTimer >= currentActionTime)
        {
            SetNewAction();
        }

        //경계 체크 및 이동
        if (Turn())
        {
            Flip();
            SetNewAction();
        }
        if (isMoving)
        {
            MoveMonster();
        }

    }

    void SetNewAction()
    {
        actionTimer = 0f;

        isMoving = Random.Range(0f, 1f) > 0.5f; //50% 확률로 이동 또는 정지

        if (isMoving)
        {
            currentActionTime = Random.Range(minMoveTime, maxMoveTime); //이동시간 설정

            if (Random.Range(0f, 1f) > 0.5f) //50% 확률로 방향 전환
            {
                Flip();
            }


        }
        else
        {
            currentActionTime = Random.Range(minStopTime, maxStopTime); //정지시간 설정
        }
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
