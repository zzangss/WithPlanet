using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("수집 목표 설정")]
    [Tooltip("아이템 획득 개수")]
    public List<int> itemCounts = new List<int>();

    [Tooltip("목표 아이템 개수")]
    public List<int> targetCounts = new List<int>();

    // View(UIManager)에 데이터 변경을 통보하는 이벤트 (DIP)
    // 인수는 (현재 카운트, 전체 카운트)
    public event Action<ItemType1, int, int> OnItemCountUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬이 변경되어도 파괴되지 않게 하려면 DontDestroyOnLoad(gameObject); 사용
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 게임 시작 시 초기 UI 상태 업데이트 통보
        for (int i = 1; i <= targetCounts.Count; i++)
        {
            // Collectable1, Collectable2, Collectable3에 해당하는 ItemType을 계산
            ItemType1 type = (ItemType1)i;
            OnItemCountUpdated?.Invoke(type, itemCounts[i - 1], targetCounts[i - 1]);
        }
    }

    // ItemPickup으로부터 획득 통보를 받는 메서드 (Controller 역할)
    public void CollectItem(ItemData data)
    {
        Debug.Log($"아이템 획득: {data.itemName} ({data.type})");

        // Collectable1, 2, 3이 Enum의 1, 2, 3 
        if (data.type >= ItemType1.Collectable1 && data.type <= ItemType1.Collectable3)
        {
            HandleTargetCollectable(data.type);
        }
        else if (data.type == ItemType1.None) // None 타입은 일반적으로 획득 로직이 없습니다.
        {
            Debug.LogWarning("None 타입 아이템은 획득 로직이 없습니다.");
        }
        else
        {
            Debug.LogWarning($"처리되지 않은 아이템 타입: {data.type}");
        }
    }

    private void HandleTargetCollectable(ItemType1 type)
    {
        // Enum 값을 인덱스로 변환
        int index = (int)type - 1;

        // 배열의 범위를 벗어나는지 확인
        if (index < 0 || index >= itemCounts.Count)
        {
            Debug.LogError($"ItemType1.{type}에 대한 카운트 인덱스가 범위를 벗어납니다.");
            return;
        }

        // 아이템 카운트 증가
        itemCounts[index]++;

        int current = itemCounts[index];
        int target = targetCounts[index];

        // UI에 업데이트 통보
        OnItemCountUpdated?.Invoke(type, current, target);

        if (current >= target)
        {
            Debug.Log($" {type} 목표 수집 완료!");
        }

    }

}
