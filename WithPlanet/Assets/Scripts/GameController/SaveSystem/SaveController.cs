using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{

    private string saveLocation;
    private PlayerItemManager pim;
    private ItemSpawner itemSpawner;

    // Start is called before the first frame update
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "savefile.json");
        pim = FindObjectOfType<PlayerItemManager>();
        itemSpawner = FindObjectOfType<ItemSpawner>();
        
    }

    // Update is called once per frame
    public void SaveGame()
    {
        SaveData saveData = new SaveData();
        //플레이어 위치 저장
        saveData.playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        //플레이어 아이템 저장
        saveData.heldItemID = pim.hasItem ? pim.currentItem.Item.ItemID : -1; //아이템 있을떄만 아이템 ID 저장, 없으면 -1

        //월드 아이템 저장
        saveData.worldItems = new List<WorldItemData>();

        WorldItem[] allWorldItems = FindObjectsOfType<WorldItem>();

        foreach (var worldItem in allWorldItems)
        {
            if (worldItem.itemLocation == ItemLocation.World)
            {
                WorldItemData itemData = new WorldItemData
                {
                    itemID = worldItem.Item.ItemID,
                    position = worldItem.transform.position
                };
                saveData.worldItems.Add(itemData);
            }
        }
        
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log("게임세이브 완료");
    }



    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            //플레이어 위치 불러오기
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;

            //플레이어 아이템 불러오기
            pim.setItem(saveData.heldItemID);

            //월드 아이템 불러오기
            if (saveData.worldItems != null && saveData.worldItems.Count > 0)
            {
                itemSpawner.clearWorld();

                foreach (var itemData in saveData.worldItems)
                {
                    itemSpawner.spawnItemToWorld(itemData.itemID, itemData.position);
                }
            }
        }
        else
        {
            Debug.Log("현재 세이브파일이 존재하지않습니다.");
            //SaveGame();
        }

    }

    //세이브 파일 삭제
    public void DeleteSaveFile()
    {
        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
            Debug.Log("세이브 파일이 삭제되었습니다.");
            LoadGame();
        }
        else
        {
            Debug.Log("삭제할 세이브 파일이 없습니다.");
        }
    }
}
