using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public TextMeshProUGUI[] itemTexts;

    private void Start()
    {
        // ItemManager의 이벤트 구독
        ItemManager.Instance.OnItemCountUpdated += UpdateItemUI;
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 구독 해제 (메모리 누수 방지)
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.OnItemCountUpdated -= UpdateItemUI;
        }
    }

    // 이벤트가 발생할 때 실행될 함수
    private void UpdateItemUI(ItemType1 type, int currentCount, int targetCount)
    {
        // Enum 값(1,2,3)을 배열 인덱스(0,1,2)로 변환
        int index = (int)type - 1;

        if (index >= 0 && index < itemTexts.Length)
        {
            // 예: "동전 : 1 / 5" 형태로 텍스트 갱신
            itemTexts[index].text = $"{currentCount} / {targetCount}";

            // (선택) 목표 달성 시 텍스트 색상 변경 효과 등
            if (currentCount >= targetCount)
            {
                itemTexts[index].color = Color.green;
            }
        }
    }
}
