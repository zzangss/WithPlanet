using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ������ �����մϴ�. 
/// 1.���� �������� ��ȣ
/// 2.���� ���������� ��ǥ ��ġ
/// 3.�������� ���� �� �Ϸ� ����
/// 4.�÷��̾��� īƮ ��ġ ���� ����
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private int currentStageNumber = 0; // ���� �������� ��ȣ
    [SerializeField] private int stageGoalValue = 1000;  // ���� �������� ��ǥ ��ġ
    [SerializeField] private int currentCartValue = 0;   // �÷��̾� īƮ�� ���� ��ġ (�̺�Ʈ�� ������Ʈ��)
                                                         // GameManager ���� �� StageManager�� �����Ͽ� ���������� ���۽�ų �� ȣ���� �� �ִ� �޼���

    private bool isCleared = false;
    public void StartGameSequence()
    {
        // ���� ���� �� �ʱ� ���������� �����մϴ�.
        StartStage(1);
    }

    /// <summary>
    /// ������ �������� ��ȣ�� �� ���������� �����մϴ�.
    /// </summary>
    
    public void StartStage(int stageNumber)
    {
        currentStageNumber = stageNumber;
        SetStageGoalValue(stageNumber); // �������� ��ȣ�� ���� ��ǥ ��ġ ����
        currentCartValue = 0; // �� �������� ���� �� īƮ ��ġ �ʱ�ȭ

        // GameEvent�� ���� �������� ���� �̺�Ʈ�� �˸��ϴ�.
        GameEvent.RaiseOnStageStart(currentStageNumber);

        Debug.Log($"StageManager: Stage {currentStageNumber} ����! ��ǥ ��ġ: {stageGoalValue}");
    }
    /// <summary>
    /// �������� ��ȣ�� ���� ��ǥ ��ġ�� �����ϴ� ���� �޼��� (���߿� �����͸� ���� �ε��� �� �ֽ��ϴ�).
    /// </summary>
    /// <param name="stageNumber"></param>
    private void SetStageGoalValue(int stageNumber)
    {
        // ����: �������� 1�� 1000, �������� 2�� 2000, �������� 3�� 3000
        stageGoalValue = 1000 * stageNumber;
        // ���� ���ӿ����� ScriptableObject�� JSON ���� ������ �����͸� �����ϴ� ���� �����ϴ�.
    }

    // -- �̺�Ʈ ���� �� ���� --
    void OnEnable()
    {
        // �÷��̾� īƮ�� ��ġ ���� �̺�Ʈ�� �����մϴ�.
        GameEvent.OnCartValueChanged += HandleCartValueChanged;
        // �÷��̾ ȸ�� �Ϸ� �� �� �̺�Ʈ ����
        GameEvent.OnTryCompleted += CheckStageCompletion;
        // GameEvent.OnPlayerDied += HandlePlayerDied; // �÷��̾� ��� �̺�Ʈ�� �����Ͽ� �������� ���� �� ó�� ����
        //player ��� �̺�Ʈ
        GameEvent.OnPlayerDied += GameOver;
    }
    void OnDisable()
    {
        // ������Ʈ�� ��Ȱ��ȭ�ǰų� �ı��� �� ������ �����մϴ�.
        GameEvent.OnCartValueChanged -= HandleCartValueChanged;
        GameEvent.OnTryCompleted -= CheckStageCompletion;
        GameEvent.OnPlayerDied -= GameOver;
    }

    /// <summary>
    /// GameEvent.OnCartValueChanged �̺�Ʈ�� �߻����� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="newCartValue">����� īƮ�� �� ��ġ</param>
    private void HandleCartValueChanged(int newCartValue)
    {
        currentCartValue = newCartValue;
        Debug.Log($"StageManager: īƮ ��ġ ������Ʈ -> {currentCartValue} / {stageGoalValue}");
       
        IsStageCompletion();
    }

    //���� �޼��ߴ��� Ȯ��
    public void IsStageCompletion()
    {
        if (currentCartValue >= stageGoalValue)
        {
            isCleared = true;
            //�̺�Ʈ ȣ�� ����, ���� ������ �����Ǿ��� �� Ư�� �̺�Ʈ�� �߻����Ѿ��Ѵٸ�
            GameEvent.RaiseOnValueSatisfied(isCleared); //���� �̺�Ʈ 
        }
    }

    /// <summary>
    /// ��ǰ ȸ�� ����(CollectionPoint)���� �÷��̾ ��ȣ�ۿ����� �� ȣ��˴ϴ�.
    /// �Ǵ� īƮ ��ġ�� Ư�� ������ �������� �� �ڵ����� ȣ��� �� �ֽ��ϴ�.
    /// </summary>
    public void CheckStageCompletion()
    {
        // ���� īƮ ��ġ�� ��ǥ ��ġ �޼� Ȯ��
        if (isCleared)
        {
            Debug.Log($"StageManager: Stage {currentStageNumber} �Ϸ�!");
            // �������� �Ϸ� �̺�Ʈ�� �߻���ŵ�ϴ�.
            GameEvent.RaiseOnStageComplete();

            // ���� ���������� ��������, ���� Ŭ��� ���� ���� ������ GameManager���� ó���� ���Դϴ�.
        }
        else
        {
            Debug.Log($"StageManager: ���� ��ǥġ �̴� ({currentCartValue} / {stageGoalValue})");
        }
    }

    //player hp==0 �϶�, stage over ó��
    public void GameOver()
    {
        GameEvent.RaiseOnGameOver();
    }

    // (���� ����) �÷��̾� ��� �� ���������� ��� ó������
    // private void HandlePlayerDied()
    // {
    //     Debug.Log("StageManager: �÷��̾� ���! �������� �ʱ�ȭ �Ǵ� ���� ���� ���.");
    //     // ���⼭�� �ٷ� ���� ���������� �Ѿ�� ���, ���� ���� ���¸� ��ٸ��ų�
    //     // ���� ���������� ������ϴ� ���� ������ ������ �� �ֽ��ϴ�.
    // }

}
