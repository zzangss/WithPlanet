using UnityEngine;

public class CCTVBillboard : MonoBehaviour
{
    public Transform target; // Player의 CCTV_TargetPoint

    void LateUpdate()
    {
        if (!target) return;

        // 대상 방향으로 회전 (Y축만)
        Vector3 dir = target.position - transform.position;
        dir.y = 0; // 수평으로만 회전하게
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}
