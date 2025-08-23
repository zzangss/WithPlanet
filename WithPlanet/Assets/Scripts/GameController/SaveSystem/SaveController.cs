using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{

    private string saveLocation;
    private PlayerItemManager pim;
    private ItemSpawner itemSpawner;
    private InventoryMain inventoryMain;
    private ItemDictionary itemdic;
    private RandomSpawner randomSpawner;

    // Start is called before the first frame update
    void Awake()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "savefile.json");
        pim = FindObjectOfType<PlayerItemManager>();
        itemSpawner = FindObjectOfType<ItemSpawner>();
        inventoryMain = FindObjectOfType<InventoryMain>();
        itemdic = FindObjectOfType<ItemDictionary>();
        randomSpawner = FindObjectOfType<RandomSpawner>();
    }

    // Update is called once per frame
    public void SaveGame()
    {
        SaveData saveData = new SaveData();
        //플레이어 위치 저장
        saveData.playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        //플레이어 체력 저장
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();


        //아이템 처리
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

        //카트 위치 저장
        saveData.cartPosition = GameObject.FindGameObjectWithTag("Cart").transform.position;

        //카트 아이템 저장
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
                        slotIndex = i // 현재 슬롯의 인덱스를 저장
                    };
                    saveData.cartItems.Add(itemData);
                }
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

            //플레이어 체력 불러오기
            PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();


            //아이템 처리
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
            // 카트 위치 불러오기
            GameObject.FindGameObjectWithTag("Cart").transform.position = saveData.cartPosition;

            // 카트 아이템 불러오기
            if (saveData.cartItems != null && saveData.cartItems.Count > 0 && inventoryMain != null)
            {
                inventoryMain.ClearAllSlots();

                foreach (var itemData in saveData.cartItems)
                {
                    GameObject itemPrefab =itemdic.GetItemPrefab(itemData.itemID);
                    Item item = itemPrefab.GetComponent<WorldItem>().Item;

                  
                    if (item !=null)
                    {
                        inventoryMain.SetItemSlot(itemData.slotIndex, item, itemData.itemCount);
                    }
                    
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

    // 저장 파일의 존재 여부를 반환하는 함수
    public bool HasSaveFile()
    {
        return File.Exists(saveLocation);
    }

    // 월드 초기화
    public void initializeWorld()
    {
       
        // 플레이어 아이템 초기화
        pim.setItem(-1); // -1로 설정하여 아이템 없음 상태로 초기화

        // 월드 아이템 초기화
        itemSpawner.clearWorld();
        
        // 카트 아이템 초기화
        if (inventoryMain != null)
        {
            inventoryMain.ClearAllSlots();
        }

        //월드 아이템 새로 생성
        randomSpawner.SpawnItems();

        Debug.Log("게임이 초기화되었습니다.");
    }
}
