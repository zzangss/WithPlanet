using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NaviMonster : MonoBehaviour
{
    public Transform target;

    [Header("Chase Settings")]
    public float chaseSpeed = 8f;
    public float detectionRange = 10f;
    public float maxChaseRange = 15f;

    [Header("Return Settings")]
    public float returnSpeed = 5f;
    public float returnRange = 2f;

    private Vector3 originalPosition;
    private bool isChasing = false;
    private bool isReturning = false;
    private NavMeshAgent nav;

    void Start()
    {
        originalPosition = transform.position;
        nav = GetComponent<NavMeshAgent>();

        // 즉시 활성화 시도
        if (nav != null)
        {
            nav.enabled = true;
            nav.updateRotation = false;
            nav.updateUpAxis = false;
            nav.acceleration = 20f;
            nav.stoppingDistance = 0.5f;
            nav.speed = chaseSpeed;

            Debug.Log("NavMeshAgent 즉시 활성화 시도");

            // NavMesh 위에 있는지 확인
            if (nav.isOnNavMesh)
            {
                Debug.Log(" NavMeshAgent 활성화 성공!");
            }
            else
            {
                Debug.LogWarning(" NavMesh 위에 없음 - 위치 조정 시도");
                StartCoroutine(FixPosition());
            }
        }
        else
        {
            Debug.LogError(" NavMeshAgent 컴포넌트를 찾을 수 없습니다!");
        }
    }

    System.Collections.IEnumerator FixPosition()
    {
        nav.enabled = false;
        yield return new WaitForEndOfFrame();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            originalPosition = hit.position;
            yield return new WaitForEndOfFrame();

            nav.enabled = true;
            Debug.Log(" 위치 보정 후 NavMeshAgent 활성화 성공!");
        }
        else
        {
            Debug.LogError(" 근처에 NavMesh를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        // 기본 조건 체크 (간단하게)
        if (nav == null || !nav.enabled || !nav.isOnNavMesh || target == null)
        {
            return; // 조건이 안 맞으면 그냥 리턴
        }

        // 거리 계산
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float distanceToOriginal = Vector3.Distance(transform.position, originalPosition);

        // 상태 전환
        if (!isChasing && !isReturning && distanceToTarget <= detectionRange)
        {
            Debug.Log(" 추적 시작!");
            StartChasing();
        }
        else if (isChasing && distanceToTarget > maxChaseRange)
        {
            Debug.Log(" 추적 포기!");
            StopChasingAndReturn();
        }
        else if (isReturning && distanceToOriginal <= returnRange)
        {
            Debug.Log(" 복귀 완료!");
            FinishReturning();
        }

        // 행동 실행
        if (isChasing)
        {
            nav.speed = chaseSpeed;
            Vector3 targetPos = target.position;
            targetPos.y = transform.position.y;
            nav.SetDestination(targetPos);
        }
        else if (isReturning)
        {
            nav.speed = returnSpeed;
            nav.SetDestination(originalPosition);
        }
    }

    void StartChasing()
    {
        isChasing = true;
        isReturning = false;
    }

    void StopChasingAndReturn()
    {
        isChasing = false;
        isReturning = true;
    }

    void FinishReturning()
    {
        isReturning = false;
        nav.ResetPath();
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = Application.isPlaying ? originalPosition : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, maxChaseRange);
    }
}