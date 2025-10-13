using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 다음을 관리합니다. 
/// 1.현재 스테이지 번호
/// 2.현재 스테이지의 목표 가치
/// 3.스테이지 시작 및 완료 로직
/// 4.플레이어의 카트 가치 변경 감지
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private int currentStageNumber = 0; // 현재 스테이지 번호
    [SerializeField] private int stageGoalValue = 1000;  // 현재 스테이지 목표 가치
    [SerializeField] private int currentCartValue = 0;   // 플레이어 카트의 현재 가치 (이벤트로 업데이트됨)
                                                         // GameManager 등이 이 StageManager를 참조하여 스테이지를 시작시킬 때 호출할 수 있는 메서드

    private bool isCleared = false;
    public void StartGameSequence()
    {
        // 게임 시작 시 초기 스테이지를 시작합니다.
        StartStage(1);
    }

    /// <summary>
    /// 지정된 스테이지 번호로 새 스테이지를 시작합니다.
    /// </summary>
    
    public void StartStage(int stageNumber)
    {
        currentStageNumber = stageNumber;
        SetStageGoalValue(stageNumber); // 스테이지 번호에 따라 목표 가치 설정
        currentCartValue = 0; // 새 스테이지 시작 시 카트 가치 초기화

        // GameEvent를 통해 스테이지 시작 이벤트를 알립니다.
        GameEvent.RaiseOnStageStart(currentStageNumber);

        Debug.Log($"StageManager: Stage {currentStageNumber} 시작! 목표 가치: {stageGoalValue}");
    }
    /// <summary>
    /// 스테이지 번호에 따라 목표 가치를 설정하는 내부 메서드 (나중에 데이터를 통해 로드할 수 있습니다).
    /// </summary>
    /// <param name="stageNumber"></param>
    private void SetStageGoalValue(int stageNumber)
    {
        // 예시: 스테이지 1은 1000, 스테이지 2는 2000, 스테이지 3은 3000
        stageGoalValue = 1000 * stageNumber;
        // 실제 게임에서는 ScriptableObject나 JSON 파일 등으로 데이터를 관리하는 것이 좋습니다.
    }

    // -- 이벤트 구독 및 해제 --
    void OnEnable()
    {
        // 플레이어 카트의 가치 변경 이벤트를 구독합니다.
        GameEvent.OnCartValueChanged += HandleCartValueChanged;
        // 플레이어가 회수 하려 할 때 이벤트 구독
        GameEvent.OnTryCompleted += CheckStageCompletion;
        // GameEvent.OnPlayerDied += HandlePlayerDied; // 플레이어 사망 이벤트도 구독하여 스테이지 리셋 등 처리 가능
    }
    void OnDisable()
    {
        // 오브젝트가 비활성화되거나 파괴될 때 구독을 해제합니다.
        GameEvent.OnCartValueChanged -= HandleCartValueChanged;
        GameEvent.OnTryCompleted -= CheckStageCompletion;
    }

    /// <summary>
    /// GameEvent.OnCartValueChanged 이벤트가 발생했을 때 호출됩니다.
    /// </summary>
    /// <param name="newCartValue">변경된 카트의 총 가치</param>
    private void HandleCartValueChanged(int newCartValue)
    {
        currentCartValue = newCartValue;
        Debug.Log($"StageManager: 카트 가치 업데이트 -> {currentCartValue} / {stageGoalValue}");
       
        IsStageCompletion();
    }

    //조건 달성했는지 확인
    public void IsStageCompletion()
    {
        if (currentCartValue >= stageGoalValue)
        {
            isCleared = true;
            //이벤트 호출 가능, 만약 조건이 충족되었을 때 특정 이벤트를 발생시켜야한다면
            GameEvent.RaiseOnValueSatisfied(isCleared); //만족 이벤트 
        }
    }

    /// <summary>
    /// 물품 회수 지점(CollectionPoint)에서 플레이어가 상호작용했을 때 호출됩니다.
    /// 또는 카트 가치가 특정 조건을 만족했을 때 자동으로 호출될 수 있습니다.
    /// </summary>
    public void CheckStageCompletion()
    {
        // 현재 카트 가치가 목표 가치 이상인지 확인
        if (currentCartValue >= stageGoalValue)
        {
            Debug.Log($"StageManager: Stage {currentStageNumber} 완료!");
            // 스테이지 완료 이벤트를 발생시킵니다.
            GameEvent.RaiseOnStageComplete();

            // 다음 스테이지를 시작할지, 게임 클리어를 할지 등의 로직은 GameManager에서 처리할 것입니다.
        }
        else
        {
            Debug.Log($"StageManager: 아직 목표치 미달 ({currentCartValue} / {stageGoalValue})");
        }
    }

    // (선택 사항) 플레이어 사망 시 스테이지를 어떻게 처리할지
    // private void HandlePlayerDied()
    // {
    //     Debug.Log("StageManager: 플레이어 사망! 스테이지 초기화 또는 게임 오버 대기.");
    //     // 여기서는 바로 다음 스테이지로 넘어가는 대신, 게임 오버 상태를 기다리거나
    //     // 현재 스테이지를 재시작하는 등의 로직을 구현할 수 있습니다.
    // }

}
