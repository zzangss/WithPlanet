using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamanagerTest : MonoBehaviour
{
    void OnEnable()
    {
        GameEvent.OnPlayerDied += HandlePlayerDied; // 플레이어 사망 이벤트 구독
    }

    void OnDisable()
    {
        
        GameEvent.OnPlayerDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        Debug.Log("GameManager: 플레이어 사망 신호 받음! 게임 오버 처리 시작...");
        GameEvent.RaiseOnGameOver();// 게임 오버 이벤트를 발생

    }
}
