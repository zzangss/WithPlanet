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
   
    [Tooltip("CCTV가 최대로 기울 수 있는 상하 각도")]
    [Range(0f, 90f)]
    public float maxVerticalAngle = 80f;

    [Header("랜덤 감시 설정")]
    [Tooltip("목표 지점에 도착한 후 대기하는 시간 (최소, 최대)")]
    public Vector2 patrolWaitTimeRange = new Vector2(1f, 3f);
    [SerializeField] private Transform[] patrolTargets;

    // 내부 변수
    private Transform playerTarget;
    private bool isPlayerDetected = false;
    private Quaternion initialRotation;
    private SpriteRenderer cctvSprite;
    //private SphereCollider detectionCollider;
    private Coroutine patrolCoroutine;
    private Vector3 initialAimerLocalPosition;


    void Awake()
    {
        cctvSprite = GetComponentInChildren<SpriteRenderer>();
        //detectionCollider = GetComponent<SphereCollider>();

        if (cctvSprite == null)
            Debug.LogError("자식 오브젝트에서 SpriteRenderer를 찾을 수 없습니다! 구조를 확인해주세요.", this.gameObject);
        if (aimerTransform == null)
            Debug.LogError("Aimer Transform이 인스펙터에 연결되지 않았습니다! 'CCTV_Aimer' 자식 오브젝트를 만들어 연결해주세요.", this.gameObject);
    }

    void Start()
    {
        initialRotation = transform.rotation;
        //detectionCollider.isTrigger = true;
        initialAimerLocalPosition = aimerTransform.localPosition;
        StartPatrol();
    }

    void Update()
    {
        /*if (isPlayerDetected && playerTarget != null)
        {
            TrackTarget(playerTarget.position);
        }*/
    }

    void TrackTarget(Vector3 targetPosition)
    {
        // 1.[조준] Aimer 조준
        Quaternion targetAimerRotation = Quaternion.LookRotation(targetPosition - aimerTransform.position);
        aimerTransform.rotation = Quaternion.Slerp(aimerTransform.rotation, targetAimerRotation, rotationSpeed * Time.deltaTime);

        // 2. [pivot 이동]Pivot과 Sprite를 제어.

        // 2-1. 스프라이트를 flip해서 오른쪽, 왼쪽을 보는것처럼 표현
        Vector3 directionToTarget = targetPosition - transform.position;
        float dotProduct = Vector3.Dot(directionToTarget, transform.right); //내적
        cctvSprite.flipX = dotProduct < 0;
        
        // 2.2 flip된 aimer에 맞춰 aimer위치도 변경해준다.  
        if (cctvSprite.flipX)
        {
         
            aimerTransform.localPosition = new Vector3(
                -initialAimerLocalPosition.x,
                -initialAimerLocalPosition.y,
                -initialAimerLocalPosition.z
            );
           
        }
        else
        {
            // flip이 아니라면 Aimer도 원래 위치
            aimerTransform.localPosition = initialAimerLocalPosition;
        }

        //상하회전
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

        // 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
    }


    IEnumerator PatrolRoutine()
    {
        if(patrolTargets ==null ||patrolTargets.Length == 0)
        {
            Debug.LogWarning("감시목표가없습니다!");
            yield break;
        }

        while(true)
        {
            //Debug.Log("목표설정");
            //1. 랜덤으로 목표 선택
            int randomIndex = Random.Range(0, patrolTargets.Length);
            Transform randomTargets = patrolTargets[randomIndex];
            Vector3 targetPos = randomTargets.position;

            //2. 
            Vector3 directionToTarget = targetPos - aimerTransform.position;
            while (Vector3.Angle(aimerTransform.forward, directionToTarget.normalized) > 5f)
            {
                if (isPlayerDetected) yield break; // 중간에 플레이어가 감지되면 즉시 중단
                TrackTarget(targetPos);

                //다음 프레임에 방향 계산
                directionToTarget = targetPos - aimerTransform.position;
                yield return null;
            }

            //3.목표지점 도착 대기
            float waitTime= Random.Range(patrolWaitTimeRange.x,patrolWaitTimeRange.y);
            yield return new WaitForSeconds(waitTime);


        }
    }

   
    /*void OnTriggerEnter(Collider other)
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
    }*/

    void StartPatrol()
    {
       // Debug.Log("감시 시작! ");
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
   
}