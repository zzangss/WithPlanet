using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{

    private string saveLocation;

    // Start is called before the first frame update
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "savefile.json");
        LoadGame();
    }

    // Update is called once per frame
    public void SaveGame()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var pim = player.GetComponent<PlayerItemManager>();

        var data = new SaveData
        {
            playerPosition = player.transform.position,
            heldItemExists = pim.hasItem,
            heldItemID = (pim.hasItem && pim.currentItem != null) ? pim.currentItem.Item.ItemID : 0
        };

        SaveData saveData = new SaveData()
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position
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

        }
        else
        {
            SaveGame();
        }

    }
}
