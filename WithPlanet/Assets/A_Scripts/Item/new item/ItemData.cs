using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 에디터에서 생성 가능하도록 설정
[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    //아이템 분류 
    public ItemType1 type = ItemType1.None;

    //표시용 정보 (Item Manager의 UI 통보)
    public string itemName = "새 아이템";
    public Sprite itemIcon;
}
