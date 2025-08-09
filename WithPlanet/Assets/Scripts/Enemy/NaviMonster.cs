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

        // ��� Ȱ��ȭ �õ�
        if (nav != null)
        {
            nav.enabled = true;
            nav.updateRotation = false;
            nav.updateUpAxis = false;
            nav.acceleration = 20f;
            nav.stoppingDistance = 0.5f;
            nav.speed = chaseSpeed;

            Debug.Log("NavMeshAgent ��� Ȱ��ȭ �õ�");

            // NavMesh ���� �ִ��� Ȯ��
            if (nav.isOnNavMesh)
            {
                Debug.Log(" NavMeshAgent Ȱ��ȭ ����!");
            }
            else
            {
                Debug.LogWarning(" NavMesh ���� ���� - ��ġ ���� �õ�");
                StartCoroutine(FixPosition());
            }
        }
        else
        {
            Debug.LogError(" NavMeshAgent ������Ʈ�� ã�� �� �����ϴ�!");
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
            Debug.Log(" ��ġ ���� �� NavMeshAgent Ȱ��ȭ ����!");
        }
        else
        {
            Debug.LogError(" ��ó�� NavMesh�� ã�� �� �����ϴ�!");
        }
    }

    void Update()
    {
        // �⺻ ���� üũ (�����ϰ�)
        if (nav == null || !nav.enabled || !nav.isOnNavMesh || target == null)
        {
            return; // ������ �� ������ �׳� ����
        }

        // �Ÿ� ���
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float distanceToOriginal = Vector3.Distance(transform.position, originalPosition);

        // ���� ��ȯ
        if (!isChasing && !isReturning && distanceToTarget <= detectionRange)
        {
            Debug.Log(" ���� ����!");
            StartChasing();
        }
        else if (isChasing && distanceToTarget > maxChaseRange)
        {
            Debug.Log(" ���� ����!");
            StopChasingAndReturn();
        }
        else if (isReturning && distanceToOriginal <= returnRange)
        {
            Debug.Log(" ���� �Ϸ�!");
            FinishReturning();
        }

        // �ൿ ����
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