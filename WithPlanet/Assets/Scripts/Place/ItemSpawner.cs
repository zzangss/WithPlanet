using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject mPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Quaternion rotation = Quaternion.Euler(eulerAngles);
        for (int i = 0; i < itemCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), 0f, Random.Range(-0.3f, 0.3f));
            Vector3 dropPosition = mPlayer.transform.position + mPlayer.transform.forward * (1.0f + i * 0.1f) + randomOffset;
            dropPosition.y += 0.5f;

            Instantiate(prefab, dropPosition, rotation);
        }

        
    }
}
