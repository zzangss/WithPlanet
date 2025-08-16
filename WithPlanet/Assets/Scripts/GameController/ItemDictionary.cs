using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// 아이템을 관리하는 딕셔너리이다. 아이템의 ID는 0부터 시작, 인스펙터에서 아이템의 ID를 정한다.
/// 딕셔너리는 아이템의 ID를 키로 해당 아이템의 프리팹을 저장한다.
///<summary>
public class ItemDictionary : MonoBehaviour
{
    public List<WorldItem> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();
        //아이템 아이디는 0,1,2,3 
        foreach (WorldItem item in itemPrefabs)
        {
            itemDictionary[item.Item.ItemID] = item.gameObject;
        }

        // 등록된 딕셔너리 확인용 출력
        foreach (var kvp in itemDictionary)
        {
            Debug.Log($"[ItemDictionary] ID={kvp.Key}, Prefab={kvp.Value.name}");
        }
    }


    public GameObject GetItemPrefabs(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if(prefab == null)
        {
            Debug.LogError("아이템 프리팹을 찾을 수 없습니다: " + itemID);
        }
        return prefab;

    }
}
