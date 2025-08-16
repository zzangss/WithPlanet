using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]


public class SaveData
{
    public Vector3 playerPosition;

    // 손에 든 아이템(있을 수도, 없을 수도)
    public bool heldItemExists;
    public int heldItemID; // hasItem==true일 때만 유효

    // 월드에 놓인 아이템들
    public List<WorldItemState> worldItems = new List<WorldItemState>();
}
