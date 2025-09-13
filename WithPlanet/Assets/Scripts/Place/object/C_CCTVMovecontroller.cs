using System.Collections;
using UnityEngine;

/// <summary>
/// 'Aimer' 자식 오브젝트를 이용해 3D 조준과 2D 표현을 분리한 CCTV 컨트롤러입니다.
/// '볼 조인트'처럼 자유로운 추적이 가능하며, 움직임이 더 부드럽고 안정적입니다.
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
    private Vector3 initialAimerLocalPosition;


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
        initialAimerLocalPosition = aimerTransform.localPosition;
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
        // 1. [조준 담당] Aimer가 목표 지점을 향해 부드럽게 회전합니다.
        Quaternion targetAimerRotation = Quaternion.LookRotation(targetPosition - aimerTransform.position);
        aimerTransform.rotation = Quaternion.Slerp(aimerTransform.rotation, targetAimerRotation, rotationSpeed * Time.deltaTime);

        // 2. [표현 담당] Aimer의 안정된 회전 정보를 이용하여 Pivot과 Sprite를 제어합니다.

        // [수정된 Flip 로직] Pivot의 로컬 오른쪽 방향을 기준으로 타겟이 왼쪽에 있는지 오른쪽에 있는지 판단하여 안정성을 높입니다.
        Vector3 directionToTarget = targetPosition - transform.position;
        float dotProduct = Vector3.Dot(directionToTarget, transform.right);
        cctvSprite.flipX = dotProduct < 0;
        
        if (cctvSprite.flipX)
        {
          
            // 스프라이트가 뒤집혔다면, Aimer의 Z 위치를 반전시켜 반대편으로 옮김
            aimerTransform.localPosition = new Vector3(
                -initialAimerLocalPosition.x,
                -initialAimerLocalPosition.y,
                -initialAimerLocalPosition.z
            );

           
        }
        else
        {
            // 스프라이트가 원래 방향이라면, Aimer도 원래 위치로 복원
            aimerTransform.localPosition = initialAimerLocalPosition;
        }

        // aimer가 바라보는 방향 벡터
        Vector3 aimerForward = aimerTransform.forward;

        // aimer 방향 벡터의 수평 투영 (Y값을 0으로 만들어 수평 방향만 남김)
        Vector3 horizontalForward = aimerForward;
        horizontalForward.y = 0;

        // 수평 방향과 실제 조준 방향 사이의 각도를 계산하여 상하 각도를 구함
        // Vector3.Angle은 항상 양수 값을 반환하므로, aimerForward.y의 부호로 위아래를 구분
        float verticalAngle = Vector3.Angle(horizontalForward, aimerForward) * Mathf.Sign(aimerForward.y);

        // 계산된 각도를 최대치로 제한
        verticalAngle = Mathf.Clamp(verticalAngle, -maxVerticalAngle, maxVerticalAngle);

        // 최종 회전값 생성: 초기 X, Y 회전은 유지하고 Z축만 Aimer의 각도로 변경
        if (cctvSprite.flipX)
        {
            verticalAngle = -verticalAngle; // 뒤집힌 상태에서는 각도도 반대로 적용
        }
        Quaternion finalRotation = Quaternion.Euler(
            initialRotation.eulerAngles.x,
            initialRotation.eulerAngles.y,
            verticalAngle // Aimer의 X축 회전(Pitch)을 Pivot의 Z축 회전(Roll)에 적용. 위로 들면(음수각도) Z가 양수가 되도록 -를 붙임.
        );

        // Pivot의 회전도 Slerp를 사용하여 더 부드럽게 만듭니다.
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

            // 목표 지점을 향한 방향 벡터를 계산
            Vector3 directionToTarget = randomTargetPosition - aimerTransform.position;
            // Aimer의 시선이 목표 방향과 거의 일치할 때까지 이동
            while (Vector3.Angle(aimerTransform.forward, directionToTarget) > 5f)
            {
                if (isPlayerDetected) yield break;
                TrackTarget(randomTargetPosition);
                // 다음 프레임까지 대기하면서 방향을 다시 계산
                directionToTarget = randomTargetPosition - aimerTransform.position;
                yield return null;
            }

            float waitTime = Random.Range(patrolWaitTimeRange.x, patrolWaitTimeRange.y);
            yield return new WaitForSeconds(waitTime);
        }
    }

    // --- 나머지 코드는 이전과 거의 동일 ---
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.cyan;
        Quaternion gizmoRotation = (Application.isPlaying) ? initialRotation : transform.rotation;
        Vector3 patrolCenter = transform.position + (gizmoRotation * Vector3.forward) * patrolAreaDistance;
        Gizmos.matrix = Matrix4x4.TRS(patrolCenter, gizmoRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(patrolAreaSize.x, patrolAreaSize.y, 0));
    }
}