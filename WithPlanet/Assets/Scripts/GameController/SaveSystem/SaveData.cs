using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]


public class SaveData
{
    public Vector3 playerPosition;
    public int heldItemID; // hasItem==true일 때만 유효
    public List<WorldItemData> worldItems; // 월드에 있는 아이템 목록
    public Vector3 cartPosition; // 카트의 위치
    public List<CartItemData> cartItems; // 카트에 있는 아이템 목록
}
