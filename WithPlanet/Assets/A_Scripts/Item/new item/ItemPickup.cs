using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Tooltip("아이템 데이터")]
    public ItemData data;


    public void ItemPickuped()
    {
        if (data == null)
        {
            Debug.LogError("ItemPickup에 ItemData가 연결되지 않았습니다!", this);
            return;
        }

        //  ItemManager에 획득 사실 통보
        ItemManager.Instance.CollectItem(data);

    }
}
