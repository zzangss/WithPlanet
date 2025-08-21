using UnityEngine;

// 이 스크립트는 2.5D 게임에서 플레이어를 부드럽게 따라가는 카메라를 만듭니다.
// 마우스 클릭으로 카메라를 회전시키고, 플레이어의 위치에 따라 부드럽게 따라갑니다.
public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("추적할 대상(플레이어)입니다.")]
    public Transform target;

    [Header("Movement Settings")]
    [Tooltip("카메라가 얼마나 부드럽게 따라갈지 설정합니다. 값이 낮을수록 더 부드럽습니다.")]
    public float smoothSpeed = 0.125f;

    [Header("Camera Offset Controls")]
    [Tooltip("카메라의 좌우(X) 위치를 조절합니다.")]
    [Range(-30f, 30f)]
    public float offsetX = 0f;

    [Tooltip("카메라의 높이(Y)를 조절합니다.")]
    [Range(0f, 30f)]
    public float offsetY = 10f;

    [Tooltip("카메라의 앞뒤(Z) 위치를 조절합니다.")]
    [Range(-30f, 0f)]
    public float offsetZ = -10f;

    [Header("Rotation Settings")]
    [Tooltip("마우스 클릭 시 카메라가 한 번에 회전할 각도입니다.")]
    public float rotationAngle = 45.0f;

    // 카메라의 오프셋(offset)입니다. 플레이어와의 상대적인 위치를 결정합니다.
    private Vector3 offset;

    // 플레이어 이동 방향 계산을 위해 카메라의 수평 방향을 저장합니다.
    public Vector3 PlanarForward { get; private set; }

    //게임 멈춤 확인
   
    void Start()
    {
        // 타겟이 설정되지 않았다면 "Player" 태그로 찾습니다.
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        // 초기 오프셋 값을 설정합니다.
        offset = new Vector3(offsetX, offsetY, offsetZ);

       
       
    }

    // LateUpdate는 모든 Update()가 호출된 후 실행되어 떨림 현상을 방지합니다.
    void LateUpdate()
    {
        // 게임이 일시정지 상태인 경우 카메라 업데이트를 건너뜁니다.
        if (PauseController.isPaused)
        {
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("Camera target is not set!");
            return;
        }

        // ===== 카메라 회전 처리 (수정된 부분) =====
        HandleRotation();

        // 1. 목표 위치 계산: 플레이어 위치 + 회전이 적용된 오프셋
        Vector3 desiredPosition = target.position + offset;

        // 2. 부드러운 이동: 현재 위치에서 목표 위치로 부드럽게 보간
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 3. 항상 타겟 바라보기
        transform.LookAt(target);

        // 4. 플레이어 이동을 위한 수평 방향 벡터 계산
        // 카메라의 전방 벡터에서 y값을 0으로 만들어 수평 방향만 남깁니다.
        Vector3 planarForward = transform.forward;
        planarForward.y = 0f;
        PlanarForward = planarForward.normalized;
    }

    /// <summary>
    /// 마우스 입력에 따라 카메라의 오프셋을 회전시킵니다.
    /// </summary>
    void HandleRotation()
    {
        float angleToRotate = 0f;

        // 마우스 오른쪽 버튼 클릭 시: 시계 방향 회전
        if (Input.GetMouseButtonDown(1)) // 1 = 오른쪽 버튼
        {
            angleToRotate = rotationAngle;
        }
        // 마우스 왼쪽 버튼 클릭 시: 반시계 방향 회전
        else if (Input.GetMouseButtonDown(0)) // 0 = 왼쪽 버튼
        {
            angleToRotate = -rotationAngle;
        }

        // 회전이 필요한 경우에만 계산을 수행합니다.
        if (angleToRotate != 0f)
        {
            // Y축을 기준으로 회전(Quaternion)을 생성합니다.
            Quaternion rotation = Quaternion.Euler(0, angleToRotate, 0);

            // 현재 오프셋에 회전을 적용하여 새로운 오프셋을 계산합니다.
            offset = rotation * offset;
        }
    }
}