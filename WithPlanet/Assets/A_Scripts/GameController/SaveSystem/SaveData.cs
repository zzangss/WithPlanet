using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]


public class SaveData
{
    //플레이어 정보
    public Vector3 playerPosition; // 플레이어의 위치
    public float playerHealth; // 플레이어의 체력


    //아이템 처리
    public int heldItemID; // hasItem==true일 때만 유효
    public List<WorldItemData> worldItems; // 월드에 있는 아이템 목록
    public Vector3 cartPosition; // 카트의 위치
    public List<CartItemData> cartItems; // 카트에 있는 아이템 목록

    //save UI 정보
    public float playTime; // 총 플레이 시간
    public string lastSavedDate; // 마지막 저장 날짜


    //게임 state 정보
    public State dialogueState; // 대화 진행 상태
    public int miniStateIdx; // 미니게임 진행 상태 인덱스
    public int currentStage; // 현재 스테이지 정보

    //게임 scene 정보
    public string sceneName;
}
