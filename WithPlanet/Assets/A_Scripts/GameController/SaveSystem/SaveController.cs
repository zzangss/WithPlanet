using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement; // [필수] 씬 관리를 위해 추가
using Project.Minigames.ToxicCleanser;

public class SaveController : MonoBehaviour
{
    [SerializeField] private PlayerAction pim;
    [SerializeField] private GameObject player;
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private InventoryMain inventoryMain;
    [SerializeField] private ItemDictionary itemdic;
    [SerializeField] private RandomSpawner[] randomSpawners;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private MinigameLauncher minigameLauncher;

    void Awake()
    {
        FindReferences(); // 참조 찾는 로직을 함수로 분리
    }

    // 씬이 바뀌면 참조가 다 끊기므로 다시 찾아주는 함수
    private void FindReferences()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player"); // 플레이어 찾기 추가
        if (pim == null) pim = FindObjectOfType<PlayerAction>();
        if (itemSpawner == null) itemSpawner = FindObjectOfType<ItemSpawner>();
        if (inventoryMain == null) inventoryMain = FindObjectOfType<InventoryMain>();
        if (itemdic == null) itemdic = FindObjectOfType<ItemDictionary>();
        if (minigameLauncher == null) minigameLauncher = FindObjectOfType<MinigameLauncher>();

        // RandomSpawner는 씬마다 개수가 다를 수 있으므로 매번 새로 찾음
        randomSpawners = FindObjectsOfType<RandomSpawner>();
    }

    private string GetSavePath(int slotIndex)
    {
        if (slotIndex == 0) return Path.Combine(Application.persistentDataPath, "initialstate.json");
        else return Path.Combine(Application.persistentDataPath, $"savefile_{slotIndex}.json");
    }

    // 게임 저장 함수
    public void SaveGame(int slotIndex, float currentPlayTime)
    {
        string saveLocation = GetSavePath(slotIndex);
        SaveData saveData = new SaveData();

        // [추가됨] 현재 씬 이름 저장
        saveData.sceneName = SceneManager.GetActiveScene().name;

        // 플레이어 정보 저장
        if (player == null) player = GameObject.FindGameObjectWithTag("Player"); // 안전장치
        saveData.playerPosition = player.transform.position;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
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
        GameObject cart = GameObject.FindGameObjectWithTag("Cart");
        if (cart != null) saveData.cartPosition = cart.transform.position;

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

        saveData.dialogueState = dialogueManager.CurrentStage;
        saveData.miniStateIdx = minigameLauncher.sceneIndex;
        saveData.playTime = currentPlayTime;
        saveData.lastSavedDate = DateTime.Now.ToString("yyyy.MM.dd HH:mm");

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log($"슬롯 {slotIndex} 저장 완료. 씬: {saveData.sceneName}");
    }

    // 게임 불러오기 함수 (수정됨: 씬 로딩 처리)
    public void LoadGame(int slotIndex)
    {
        string saveLocation = GetSavePath(slotIndex);
        if (File.Exists(saveLocation))
        {
            string json = File.ReadAllText(saveLocation);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // [핵심] 현재 씬과 저장된 씬이 다르면 씬 이동부터 수행
            string currentScene = SceneManager.GetActiveScene().name;
            if (!string.IsNullOrEmpty(saveData.sceneName) && saveData.sceneName != currentScene)
            {
                // 코루틴을 통해 씬 로딩 대기 후 데이터 적용
                StartCoroutine(LoadSceneAndRestore(saveData.sceneName, saveData, slotIndex));
            }
            else
            {
                // 같은 씬이면 바로 적용
                RestoreGameData(saveData, slotIndex);
            }
        }
        else
        {
            Debug.Log($"슬롯 {slotIndex}에 세이브파일이 없습니다.");
        }
    }

    // 씬을 비동기로 로드하고 완료되면 데이터를 복구하는 코루틴
    private IEnumerator LoadSceneAndRestore(string sceneName, SaveData saveData, int slotIndex)
    {
        Debug.Log($"씬 이동 시작: {sceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 씬 로딩이 끝날 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 씬 로딩 직후 한 프레임 더 대기 (오브젝트 초기화 안정성 확보)
        yield return null;

        // 씬이 바뀌었으므로 플레이어, 스포너 등 참조를 다시 찾아야 함
        FindReferences();

        // 데이터 복구 실행
        RestoreGameData(saveData, slotIndex);
    }

    // 실제 데이터를 적용하는 함수 (LoadGame에서 분리됨)
    private void RestoreGameData(SaveData saveData, int slotIndex)
    {
        // 플레이어 위치 및 상태 복구
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.MovePosition(saveData.playerPosition);
                // 물리 간섭 방지를 위해 속도 초기화 권장
                rb.velocity = Vector3.zero;
            }
            else
            {
                player.transform.position = saveData.playerPosition;
            }

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.health = saveData.playerHealth;

            if (pim != null) pim.setItem(saveData.heldItemID);
        }

        // 월드 아이템 복구
        if (itemSpawner != null)
        {
            itemSpawner.clearWorld();
            if (saveData.worldItems != null)
            {
                foreach (var itemData in saveData.worldItems)
                {
                    itemSpawner.spawnItemToWorld(itemData.itemID, itemData.position);
                }
            }
        }

        // 카트 복구
        GameObject cart = GameObject.FindGameObjectWithTag("Cart");
        if (cart != null) cart.transform.position = saveData.cartPosition;

        // 인벤토리 복구
        if (inventoryMain != null)
        {
            inventoryMain.gameObject.SetActive(true);
            inventoryMain.ClearAllSlots();
            if (saveData.cartItems != null)
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
            inventoryMain.gameObject.SetActive(false);
        }

        // 기타 상태 복구
        if (dialogueManager != null) dialogueManager.CurrentStage = saveData.dialogueState;
        if (minigameLauncher != null) minigameLauncher.sceneIndex = saveData.miniStateIdx;

        // GameManagerSystem이 존재한다면 시간 복구
        if (GameManagerSystem.Instance != null)
        {
            GameManagerSystem.Instance.playTime = saveData.playTime;
        }

        Debug.Log($"슬롯 {slotIndex} 로드 완료 (씬: {saveData.sceneName})");
    }

    public void DeleteSaveFile(int slotIndex)
    {
        string saveLocation = GetSavePath(slotIndex);
        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
            Debug.Log($"슬롯 {slotIndex} 삭제됨");
        }
    }

    public bool HasSaveFile(int slotIndex)
    {
        return File.Exists(GetSavePath(slotIndex));
    }

    public float GetPlayTime(int slotIndex)
    {
        string saveLocation = GetSavePath(slotIndex);
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            return saveData.playTime;
        }
        return 0f;
    }

    public string GetLastSavedDate(int slotIndex)
    {
        string saveLocation = GetSavePath(slotIndex);
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            return saveData.lastSavedDate;
        }
        return "N/A";
    }

    public void initializeWorld()
    {
        if (itemSpawner != null) itemSpawner.clearWorld();

        if (randomSpawners.Length > 0)
        {
            foreach (var spawner in randomSpawners)
            {
                if (spawner != null) spawner.SpawnItems();
            }
        }
        Debug.Log("게임 월드 초기화");
    }

    private IEnumerator ReEnableController(CharacterController controller)
    {
        yield return null;
        if (controller != null) controller.enabled = true;
    }
}