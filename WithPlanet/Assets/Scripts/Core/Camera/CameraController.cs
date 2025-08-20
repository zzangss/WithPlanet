using UnityEngine;

/// <summary>
/// 2.5D 게임에서 대상을 부드럽게 따라가는 카메라입니다.
/// 카메라는 에디터에서 설정된 고정된 회전값을 유지합니다.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("카메라가 따라갈 대상(플레이어의 루트 오브젝트)입니다.")]
    public Transform target;

    [Tooltip("카메라가 대상을 따라가는 부드러움의 정도입니다. 높을수록 빠르게 반응합니다.")]
    public float smoothSpeed = 5f;

    [Tooltip("대상으로부터 카메라가 떨어져 있을 상대적인 위치입니다.")]
    public Vector3 offset = new Vector3(10f, 12f, -10f);

    void Start()
    {
        // 인스펙터에서 타겟이 설정되지 않았다면 "Player" 태그를 가진 오브젝트를 찾습니다.
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("'Player' 태그를 가진 대상을 찾아 타겟으로 설정했습니다.");
            }
            else
            {
                Debug.LogError("카메라가 추적할 대상을 찾을 수 없습니다. 'Player' 태그를 확인하거나 Target을 직접 할당해주세요.");
            }
        }
    }

    // 모든 Update 로직이 끝난 후 호출되어 떨림(Jitter) 현상을 방지합니다.
    void LateUpdate()
    {
        // 추적할 대상이 없으면 아무것도 하지 않습니다.
        if (target == null) return;

        // 1. 카메라가 있어야 할 목표 위치를 계산합니다.
        Vector3 desiredPosition = target.position + offset;

        // 2. 현재 위치에서 목표 위치로 부드럽게 이동합니다. (프레임 속도와 무관하게)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 3. 계산된 위치로 카메라의 위치를 업데이트합니다.
        transform.position = smoothedPosition;
    }
}