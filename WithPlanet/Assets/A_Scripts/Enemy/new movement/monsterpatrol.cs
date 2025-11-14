using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterpatrol : MonoBehaviour
{
    [Header("순찰 설정")]
    public Transform[] patrolTargets;// 몬스터가 이동할 목표
    public float moveSpeed = 3f;     // 몬스터의 이동 속도
    public float waitTime = 2f;      // 목표 지점에 도착 후 대기할 시간 (초)
    public float stoppingDistance = 0.1f; // 목표에 도착했다고 판단할 거리

    private int currentTargetIndex = 0; // 현재 목표 인덱스
    private int targetNum;             // 배열 길이
    private int moveDirection = 1;     // 1: 정방향(Up), -1: 역방향(Down)

    private Animator anim;//애니메이션
    private SpriteRenderer sr; //스프라이트
    private float initialY;//몬스터의 초기 Y 좌표


    void Start()
    {
        initialY = transform.position.y;
        targetNum = patrolTargets.Length;
         anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (targetNum > 0)
        {
            StartCoroutine(PatrolRoutine());
        }
        else
        {
            Debug.LogError("Patrol Targets 배열이 비어 있습니다. Inspector에서 목표들을 설정해주세요.");
        }
    }

    private void SetNextTarget()
    {
        currentTargetIndex += moveDirection;

        // 끝에 도달했을 경우
        if (currentTargetIndex >= targetNum ) //끝
        {
            // 마지막 인덱스에 도달했으므로 방향을 역방향(-1)으로 
            moveDirection = -1;

            currentTargetIndex = targetNum - 1;
        }
        //시작
        else if (currentTargetIndex <= 0)
        {
            // 첫 인덱스에 도달했으므로 방향을 정방향(1)으로 
            moveDirection = 1;

            currentTargetIndex = 0;
        }
    }
    /// <summary>
    /// 이동 및 대기 동작을 관리하는 코루틴
    /// </summary>
    IEnumerator PatrolRoutine()
    {
        while (true) // 무한 루프: 순찰을 계속 반복
        {
            //목표설정
            Transform target = patrolTargets[currentTargetIndex];

            //애니메이션
            if (anim != null) anim.SetBool("isMoving", true);

            // 목표 지점에 충분히 가까워질 때까지 이동
            while (Vector3.Distance(transform.position, target.position) > stoppingDistance)
            {
                // 목표 방향으로 이동
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target.position,
                    moveSpeed * Time.deltaTime
                );

                // 스프라이트 플립 (좌우 방향 설정)
                if (sr != null)
                {
                    // 목표보다 오른쪽에 있으면
                    if (target.position.x > transform.position.x)
                    {
                        sr.flipX = true;
                    }
                    // 목표보다 왼쪽에 있으면
                    else if (target.position.x < transform.position.x)
                    {
                        sr.flipX = false;
                    }
                }

                yield return null; // 다음 프레임까지 대기
            }
            // 2. 대기 페이즈 (도착)
            if (anim != null) anim.SetBool("isMoving", false); // Idle 애니메이션 전환

            Debug.Log($"목표 지점 {currentTargetIndex}에 도착했습니다. {waitTime}초 대기...");
            yield return new WaitForSeconds(waitTime);

            // 3. 다음 목표 설정
            SetNextTarget();
            Debug.Log($"다음 목표 지점 {currentTargetIndex}로 이동 준비. 방향: {(moveDirection == 1 ? "정방향" : "역방향")}");

            /*
            // 현재 인덱스를 제외한 새로운 무작위 인덱스 선택
            int newIndex = currentTargetIndex;
            while (newIndex == currentTargetIndex)
            {
                newIndex = Random.Range(0, patrolTargets.Length);
            }
            currentTargetIndex = newIndex;
                */
        }
    }





}
