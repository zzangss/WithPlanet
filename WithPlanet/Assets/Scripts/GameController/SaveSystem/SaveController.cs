using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{

    private string saveLocation;
    private PlayerItemManager pim;

    // Start is called before the first frame update
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "savefile.json");
        pim = FindObjectOfType<PlayerItemManager>();
        LoadGame();
    }

    // Update is called once per frame
    public void SaveGame()
    {
        SaveData saveData = new SaveData()
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            heldItemID = pim.hasItem ? pim.currentItem.Item.ItemID : -1, // 아이템이 있을 때만 ID  없으면 -1 저장

        };
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log("게임세이브 완료");
    }



    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;

            pim.setItem(saveData.heldItemID);
        }
        else
        {
            SaveGame();
        }

    }
}
