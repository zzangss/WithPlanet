using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 게임 전반의 중요한 이벤트들을 모아두는 정적(Static) 클래스입니다.
/// 다른 클래스들이 서로 직접 참조하지 않고 이벤트를 발행/구독할 수 있도록 돕습니다.
/// </summary>
/// 
public class GameEvent
{
    //스테이지 관련 이벤트
    //새로운 스테이지가 시작될 때 발생하는 이벤트 
    public static event Action<int> OnStageStart;
    public static void RaiseOnStageStart(int stageNum)
    {
        OnStageStart?.Invoke(stageNum);
        Debug.Log($"[GameEvents] Stage {stageNum} Started.");

    }
    //스테이지 목표에 달성했을 때 발생하는 이벤트
    public static event Action<bool> OnValueSatisfied;
    public static void RaiseOnValueSatisfied(bool isTrue)
    {
        OnValueSatisfied?.Invoke(isTrue);
        Debug.Log($"[GameEvents] Stage 목표 가치 만족! ");

    }

    //현재 스테이지가 완료되었을 때 이벤트
    public static event Action OnStageComplete;
    public static void RaiseOnStageComplete()
    {
        OnStageComplete?.Invoke();
        Debug.Log("[GameEvents] Stage Completed!");
    }

//플레이어 관련 이벤트
public static event Action OnPlayerDied;
//gameover. 플레이어의 hp가 0이 되었을 때
public static void RaiseOnPlayerDied()
    {
        OnPlayerDied?.Invoke();
        Debug.Log("[GameEvents] Player Died!");
       
    }

    // 플레이어의 카트(인벤토리)에 있는 아이템 가치가 변경되었을 때 발생하는 이벤트입니다.
    public static event Action<int> OnCartValueChanged;
    public static void RaiseOnCartValueChanged(int currentCartValue)
    {
        OnCartValueChanged?.Invoke(currentCartValue);
        Debug.Log($"[GameEvents] Cart Value Changed: {currentCartValue}");
    }

    //플레이어가 회수를 시도할 때 이벤트
    public static event Action OnTryCompleted;
    public static void RaiseOnTryComplete()
    {
        OnTryCompleted?.Invoke();
        Debug.Log($"[GameEvents] 회수 시도:");
    }

    //게임 상태 관련 이벤트

    // 게임이 시작되었음을 알리는 이벤트입니다.
    public static event Action OnGameStart;
    public static void RaiseOnGameStart()
    {
        OnGameStart?.Invoke();
        Debug.Log("[GameEvents] Game Started!");
    }

    
    // 게임 오버 상태가 되었음을 알리는 이벤트입니다.
    
    public static event Action OnGameOver;
    public static void RaiseOnGameOver()
    {
        OnGameOver?.Invoke();
        Debug.Log("[GameEvents] Game Over!");
    }
    // 게임이 일시 정지/재개 되었을 때 발생하는 이벤트입니다.
    public static event Action<bool> OnGamePaused;
    public static void RaiseOnGamePaused(bool isPaused)
    {
        OnGamePaused?.Invoke(isPaused);
        Debug.Log($"[GameEvents] Game Paused: {isPaused}");
    }

    // 플레이어 체력 변경 (UI 업데이트 등)
    public static event Action<float, float> OnPlayerHealthChanged; // currentHealth, maxHealth
    public static void RaiseOnPlayerHealthChanged(float current, float max) { OnPlayerHealthChanged?.Invoke(current, max); }

 
}
