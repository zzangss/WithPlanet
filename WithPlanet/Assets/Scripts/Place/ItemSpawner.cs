using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject mPlayer;
    [SerializeField] private ItemDictionary itemDictionary;

    // Start is called before the first frame update
    void Start()
    {
        if (mPlayer == null)
        {
            mPlayer = GameObject.FindGameObjectWithTag("Player");
            itemDictionary = FindObjectOfType<ItemDictionary>();

            if (itemDictionary == null)
            {
                Debug.LogWarning("ItemDictionary not found in the scene. Please ensure it is present.");
            }
            if (mPlayer == null)
            {
                Debug.LogWarning("Player GameObject not found. Please assign it in the inspector.");
            }
        }
    }

    public void DropItemToWorld(Item item, int itemCount)
    {
        if(item == null)
        {
            Debug.LogWarning("Item is null");
            return;
        }

        GameObject prefab = item.WorldPrefab;
        if (prefab == null)
        {
            Debug.LogWarning($"[DropItemToWorld] {item.name} 프리팹이 지정되지 않았습니다.");
            return;
        }


        Vector3 eulerAngles = new Vector3(30f, 0f, 0f);
        Quaternion rotation = Quaternion.Euler(45f, 45f, 0f);
        for (int i = 0; i < itemCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), 0f, Random.Range(-0.3f, 0.3f));
            Vector3 dropPosition = mPlayer.transform.position + mPlayer.transform.forward * (1.0f + i * 0.1f) + randomOffset;
            dropPosition.y += 0.5f;

            //1. 아이템 프리팹을 월드에 생성, 변수에 저장
            GameObject droppedItemGO = Instantiate(prefab, dropPosition, rotation);

            //2. 게임오브젝트에서 WorldItem 컴포넌트를 가져온다.
            WorldItem worldItem = droppedItemGO.GetComponent<WorldItem>();

            //3. 월드아이템의 itemLocation을 World로 설정한다.
            if(worldItem != null)
            {
                worldItem.itemLocation = ItemLocation.World;
            }
        }
    }

    //세이브 시 월드에 아이템 생성하기
    public void spawnItemToWorld(int ItemID, Vector3 position)
    {
        
        if (itemDictionary == null)
        {
            Debug.LogWarning("ItemDictionary is not assigned. Please assign it in the inspector.");
            return;
        }
        GameObject prefab = itemDictionary.GetItemPrefab(ItemID);
        if (prefab == null)
        {
            Debug.LogWarning($"[SpawnItemToWorld] 아이템 ID {ItemID}에 해당하는 프리팹을 찾을 수 없습니다.");
            return;
        }
        Quaternion rotation = Quaternion.Euler(45f, 45f, 0f);
        Instantiate(prefab, position, rotation);
    }

    //월드 아이템 제거
    public void clearWorld()
    {
        // 월드 아이템을 찾기
        WorldItem[] worlditems = FindObjectsOfType<WorldItem>();
        if (worlditems.Length == 0)
        {
            Debug.LogWarning("월드 아이템이 없습니다. 아이템을 제거하지 않습니다");
            return;
        }
      
        foreach (WorldItem item in worlditems)
        {
            if (item.itemLocation == ItemLocation.World)
            {
                Destroy(item.gameObject);
            }
        }
    }
}
