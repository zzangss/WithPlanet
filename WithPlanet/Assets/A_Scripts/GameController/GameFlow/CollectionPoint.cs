using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectionPoint : MonoBehaviour
{ 


    void OnTrigger(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("CollectionPoint: 플레이어가 회수를 시도했습니다.");
        //StageManager가 이미 OnCartValueChanged를 감시하므로,

        // 단순히 StageManager에 직접 회수 시도를 알림.

            GameEvent.RaiseOnTryComplete();
            
        }
    }
}
