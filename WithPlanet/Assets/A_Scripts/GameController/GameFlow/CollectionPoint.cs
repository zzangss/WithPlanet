using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionPoint : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("CollectionPoint: 플레이어가 회수를 시도했습니다.");
            //StageManager가 이미 OnCartValueChanged를 감시하므로,
            // 플레이어가 카트 가치를 변경하고 나서 회수 포인트에서 조건을 검사하도록 지시할 수 있음.
            // 또는 단순히 StageManager에 직접 회수 시도를 알림.


            FindObjectOfType<StageManager>()?.CheckStageCompletion();
        }
    }
}
