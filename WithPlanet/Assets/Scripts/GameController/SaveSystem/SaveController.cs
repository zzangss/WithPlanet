using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{
    private PlayerAction pim;
    private ItemSpawner itemSpawner;
    private InventoryMain inventoryMain;
    private ItemDictionary itemdic;
    private RandomSpawner randomSpawner;

    void Awake()
    {
        pim = FindObjectOfType<PlayerAction>();
        itemSpawner = FindObjectOfType<ItemSpawner>();
        inventoryMain = FindObjectOfType<InventoryMain>();
        itemdic = FindObjectOfType<ItemDictionary>();
        randomSpawner = FindObjectOfType<RandomSpawner>();
    }

    // 슬롯 번호에 따라 다른 세이브 파일 경로를 반환하는 함수
    // 0번 슬롯은 초기 상태 파일, 1~3번은 사용자 세이브 파일
    private string GetSavePath(int slotIndex)
    {
        if (slotIndex == 0)
        {
            return Path.Combine(Application.persistentDataPath, "initialstate.json");
        }
        else
        {
            return Path.Combine(Application.persistentDataPath, $"savefile_{slotIndex}.json");
        }
    }

    // 게임 저장 함수 (슬롯 번호 지정)
    public void SaveGame(int slotIndex)
    {
        string saveLocation = GetSavePath(slotIndex);
        SaveData saveData = new SaveData();

        // 플레이어 위치 저장
        saveData.playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        saveData.playerHealth = playerHealth.health;
        saveData.heldItemID = pim.hasItem ? pim.currentItem.Item.ItemID : -1;

        // 월드 아이템 저장
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

        // 카트 위치 저장
        saveData.cartPosition = GameObject.FindGameObjectWithTag("Cart").transform.position;

        // 카트 아이템 저장
        saveData.cartItems = new List<CartItemData>();
        if (inventoryMain != null)
        {
            InventorySlot[] allSlots = inventoryMain.GetAllItems();
            for (int i = 0; i < allSlots.Length; i++)
            {
                if (allSlots[i].Item != null)
                {
                    CartItemData itemData = new CartItemData
                    {
                        itemID = allSlots[i].Item.ItemID,
                        itemCount = allSlots[i].mItemCount,
                        slotIndex = i
                    };
                    saveData.cartItems.Add(itemData);
                }
            }
        }
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log($"슬롯 {slotIndex}에 게임세이브 완료");
    }

    // 게임 불러오기 함수 (슬롯 번호 지정)
    public void LoadGame(int slotIndex)
    {
        string saveLocation = GetSavePath(slotIndex);
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = saveData.playerPosition;
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.health = saveData.playerHealth;
            pim.setItem(saveData.heldItemID);

            // 월드 아이템 불러오기
            if (itemSpawner != null)
            {
                itemSpawner.clearWorld();
            }
            if (saveData.worldItems != null && saveData.worldItems.Count > 0)
            {
                foreach (var itemData in saveData.worldItems)
                {
                    itemSpawner.spawnItemToWorld(itemData.itemID, itemData.position);
                }
            }
            // 카트 위치 불러오기
            GameObject.FindGameObjectWithTag("Cart").transform.position = saveData.cartPosition;

            // 카트 아이템 불러오기
            if (inventoryMain != null)
            {
                inventoryMain.ClearAllSlots();
                if (saveData.cartItems != null && saveData.cartItems.Count > 0)
                {
                    foreach (var itemData in saveData.cartItems)
                    {
                        GameObject itemPrefab = itemdic.GetItemPrefab(itemData.itemID);
                        if (itemPrefab != null)
                        {
                            Item item = itemPrefab.GetComponent<WorldItem>().Item;
                            if (item != null)
                            {
                                inventoryMain.SetItemSlot(itemData.slotIndex, item, itemData.itemCount);
                            }
                        }
                    }
                }
            }
            Debug.Log($"슬롯 {slotIndex}에서 게임 불러오기 완료");
        }
        else
        {
            Debug.Log($"슬롯 {slotIndex}에 세이브파일이 존재하지않습니다.");
        }
    }

    // 세이브 파일 삭제 (슬롯 번호 지정)
    public void DeleteSaveFile(int slotIndex)
    {
        string saveLocation = GetSavePath(slotIndex);
        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
            Debug.Log($"슬롯 {slotIndex}의 세이브 파일이 삭제되었습니다.");
        }
        else
        {
            Debug.Log($"슬롯 {slotIndex}에는 삭제할 세이브 파일이 없습니다.");
        }
    }

    // 저장 파일의 존재 여부를 반환하는 함수 (슬롯 번호 지정)
    public bool HasSaveFile(int slotIndex)
    {
        return File.Exists(GetSavePath(slotIndex));
    }

    // 월드 초기화 (월드 아이템만 바꾸기. 나머지는 초기 상태 그대로 저장)
    public void initializeWorld()
    {
       
        //  월드 아이템 초기화
        if (itemSpawner != null)
        {
            itemSpawner.clearWorld();
        }

        // 5. 월드 아이템 새로 생성
        if (randomSpawner != null)
        {
            randomSpawner.SpawnItems();
        }
        Debug.Log("게임 월드 초기화");
    }
}