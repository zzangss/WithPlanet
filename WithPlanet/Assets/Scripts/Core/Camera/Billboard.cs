using UnityEngine;

public class Billboard : MonoBehaviour
{
    // 인스펙터 창에서 카메라를 직접 할당합니다.
    public Camera targetCamera;

    void Start()
    {
        // 카메라가 할당되지 않았다면 경고를 표시하고 기본 카메라를 찾습니다.
        if (targetCamera == null)
        {
            Debug.LogWarning("Billboard 스크립트에 Target Camera가 할당되지 않았습니다. Camera.main을 사용합니다.", this.gameObject);
            targetCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (targetCamera != null)
        {
            // 빌보드 오브젝트가 카메라의 위치(position)를 직접 바라보게 만듭니다.
            // 이렇게 하면 카메라의 회전값과 상관없이 항상 정면을 보게 됩니다.
            transform.LookAt(targetCamera.transform.position);

            // (선택사항) 빌보드가 위아래로 기울어지는게 어색하다면 아래 코드의 주석을 푸세요.
            // transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}