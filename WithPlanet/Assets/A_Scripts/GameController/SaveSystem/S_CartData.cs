using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// item 직렬화 필요
[System.Serializable]
public class CartItemData 
{
    public int itemID; // 아이템 ID
    public int itemCount; // 아이템 개수
    public int slotIndex; // 슬롯 인덱스 (0부터 시작)
}
