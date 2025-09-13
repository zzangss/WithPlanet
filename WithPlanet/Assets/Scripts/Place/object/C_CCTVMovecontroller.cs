using System.Collections;
using UnityEngine;

/// <summary>
/// 'Aimer' 자식 오브젝트를 이용해 3D 조준과 2D 표현을 분리한 CCTV 컨트롤러입니다.
/// '볼 조인트'처럼 자유로운 추적이 가능합니다.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class CCTV_AimerBased_Controller : MonoBehaviour
{
    [Header("핵심 참조")]
    [Tooltip("플레이어를 조준할 보이지 않는 자식 오브젝트 (CCTV_Aimer)")]
    public Transform aimerTransform;

    [Header("추적 설정")]
    [Tooltip("CCTV 회전속도")]
    public float rotationSpeed = 5f;
    [Tooltip("CCTV 감지범위")]
    public float detectionRadius = 15f;
    [Tooltip("CCTV가 최대로 기울 수 있는 상하 각도")]
    [Range(0f, 90f)]
    public float maxVerticalAngle = 80f;

    [Header("랜덤 감시 설정")]
    [Tooltip("순찰할 영역 크기 (가로, 세로)")]
    public Vector2 patrolAreaSize = new Vector2(10f, 5f);
    [Tooltip("순찰 영역이 CCTV로부터 얼마나 떨어져 있는지")]
    public float patrolAreaDistance = 10f;
    [Tooltip("목표 지점에 도착한 후 대기하는 시간 (최소, 최대)")]
    public Vector2 patrolWaitTimeRange = new Vector2(1f, 3f);

    // 내부 변수
    private Transform playerTarget;
    private bool isPlayerDetected = false;
    private Quaternion initialRotation;
    private SpriteRenderer cctvSprite;
    private SphereCollider detectionCollider;
    private Coroutine patrolCoroutine;

    void Awake()
    {
        cctvSprite = GetComponentInChildren<SpriteRenderer>();
        detectionCollider = GetComponent<SphereCollider>();

        if (cctvSprite == null)
            Debug.LogError("자식 오브젝트에서 SpriteRenderer를 찾을 수 없습니다! 구조를 확인해주세요.", this.gameObject);
        if (aimerTransform == null)
            Debug.LogError("Aimer Transform이 인스펙터에 연결되지 않았습니다! 'CCTV_Aimer' 자식 오브젝트를 만들어 연결해주세요.", this.gameObject);
    }

    void Start()
    {
        initialRotation = transform.rotation;
        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;
        StartPatrol();
    }

    void Update()
    {
        if (isPlayerDetected && playerTarget != null)
        {
            TrackTarget(playerTarget.position);
        }
    }

    void TrackTarget(Vector3 targetPosition)
    {
        // 1. [조준 담당] Aimer가 목표 지점을 완벽하게 3D로 조준합니다.
        aimerTransform.LookAt(targetPosition);

        // 2. [표현 담당] Aimer의 회전 정보를 이용하여 Pivot과 Sprite를 제어합니다.

        // 2-1. Flip 처리: Aimer의 현재 정면 방향과 월드 오른쪽 방향(Vector3.right)을 비교
        float dotProduct = Vector3.Dot(aimerTransform.forward, Vector3.right);
        cctvSprite.flipX = dotProduct < 0;

        // 2-2. Z축 회전 처리 (상하): Aimer의 로컬 X축 회전값을 가져와 Pivot의 Z축에 적용
        float verticalAngle = aimerTransform.localEulerAngles.x;

        // 360도 형식의 각도를 -180 ~ 180 형식으로 변환하여 자연스럽게 만듭니다.
        if (verticalAngle > 180) verticalAngle -= 360;

        verticalAngle = Mathf.Clamp(verticalAngle, -maxVerticalAngle, maxVerticalAngle);

        // 최종 회전값 생성: 초기 X, Y 회전은 유지하고 Z축만 Aimer의 각도로 변경
        Quaternion finalRotation = Quaternion.Euler(
            initialRotation.eulerAngles.x,
            initialRotation.eulerAngles.y,
            -verticalAngle // Aimer의 X축 회전(Pitch)을 Pivot의 Z축 회전(Roll)에 적용
        );

        // Pivot을 부드럽게 최종 회전값으로 이동
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // 초기 방향을 기준으로 순찰 영역을 계산
            Vector3 patrolCenter = transform.position + (initialRotation * Vector3.forward) * patrolAreaDistance;
            Vector3 patrolRight = initialRotation * Vector3.right;
            Vector3 patrolUp = initialRotation * Vector3.up;

            float randomX = Random.Range(-patrolAreaSize.x / 2, patrolAreaSize.x / 2);
            float randomY = Random.Range(-patrolAreaSize.y / 2, patrolAreaSize.y / 2);
            Vector3 randomTargetPosition = patrolCenter + patrolRight * randomX + patrolUp * randomY;

            // 목표 지점에 도착할 때까지 대략적인 시간 동안 이동
            float journeyDuration = Vector3.Distance(aimerTransform.position, randomTargetPosition) / (rotationSpeed * 2f);
            float elapsedTime = 0;
            while (elapsedTime < journeyDuration)
            {
                if (isPlayerDetected) yield break;
                TrackTarget(randomTargetPosition);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            float waitTime = Random.Range(patrolWaitTimeRange.x, patrolWaitTimeRange.y);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform targetPoint = other.transform.Find("CCTV_TargetPoint");
            if (targetPoint != null)
            {
                isPlayerDetected = true;
                playerTarget = targetPoint;
                StopPatrol();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = false;
            playerTarget = null;
            StartPatrol();
        }
    }

    void StartPatrol()
    {
        if (patrolCoroutine == null)
        {
            patrolCoroutine = StartCoroutine(PatrolRoutine());
        }
    }

    void StopPatrol()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 플레이어 감지 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 순찰 영역
        Gizmos.color = Color.cyan;
        Quaternion gizmoRotation = (Application.isPlaying) ? initialRotation : transform.rotation;
        Vector3 patrolCenter = transform.position + (gizmoRotation * Vector3.forward) * patrolAreaDistance;
        Gizmos.matrix = Matrix4x4.TRS(patrolCenter, gizmoRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(patrolAreaSize.x, patrolAreaSize.y, 0));
    }
}