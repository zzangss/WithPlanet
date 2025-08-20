using UnityEngine;

// 이 스크립트는 2.5D 게임에서 플레이어를 부드럽게 따라가는 카메라를 만듭니다.
// 카메라의 위치를 플레이어의 위치에 따라 조정하여 시야를 안정적으로 유지합니다.
public class CameraController : MonoBehaviour
{
    // 추적할 대상(플레이어)을 인스펙터 창에서 설정할 수 있습니다.
    public Transform target;

    // 카메라가 얼마나 부드럽게 따라갈지 설정합니다. 값이 낮을수록 더 부드럽습니다.
    public float smoothSpeed = 0.125f;

    // 카메라의 오프셋(offset)을 설정합니다. 
    // 등각 투영 뷰에서 플레이어와의 상대적인 위치를 결정합니다.
    public Vector3 offset = new Vector3(-12f, 12f, -12f);

    // LateUpdate는 모든 Update() 함수가 호출된 후 마지막에 호출됩니다.
    // 이는 플레이어의 이동이 먼저 처리된 후에 카메라 위치를 업데이트하기 때문에
    // "덜덜거림(jitter)" 현상을 방지합니다.

    void Start()
    {
        // 카메라가 시작할 때, 타겟이 설정되어 있는지 확인합니다.
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }
    void LateUpdate()
    {
        // 추적할 대상이 있는지 확인합니다.
        if (target == null)
        {
            Debug.LogWarning("Camera target is not set!");
            return;
        }

        // 원하는 목표 위치를 계산합니다.
        // 플레이어의 위치(target.position)에 설정된 오프셋을 더합니다.
        Vector3 desiredPosition = target.position + offset;

        // Vector3.Lerp를 사용하여 현재 카메라 위치에서 목표 위치까지 부드럽게 보간(interpolate)합니다.
        // 이로 인해 카메라가 갑자기 움직이는 것이 아니라, 부드럽게 따라가게 됩니다.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 계산된 부드러운 위치로 카메라의 위치를 업데이트합니다.
        transform.position = smoothedPosition;

        // 카메라가 항상 플레이어를 바라보도록 회전합니다.
        // 이 코드를 사용하면 카메라의 기울기를 수동으로 설정할 필요 없이 플레이어를 향하게 됩니다.
        transform.LookAt(target);
    }

}