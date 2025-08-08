using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Flags]
public enum ItemType  // 아이템 유형
{
    NONE = 0b0,
    NORMAL = 0b1, //1
    CORE = 0b10 //2
}

[CreateAssetMenu(fileName = "Item", menuName =  "Add Item/Item")]
public class Item : ScriptableObject
{
    [Header("고유한 아이템의 ID(중복불가)")]
    [SerializeField] private int mItemID;
    public int ItemID
    {
        get
        {
            return mItemID;
        }
    }

    [Header("아이템의 중첩이 가능한가?")]
    [SerializeField] private bool mCanOverlap;
    public bool CanOverlap
    {
        get
        {
            return mCanOverlap;
        }
    }

    [Header("아이템의 타입")]
    [SerializeField] private ItemType mItemType;

    public ItemType Type
    {
        get
        {
            return mItemType;
        }
    }

    [Header("아이템의 이름")]
    [SerializeField] private string mName;

    public string Name
    {
        get
        {
            return mName;
        }
    }

    [Header("아이템의 이미지")]
    [SerializeField] private Sprite mItemImage;
    public Sprite Image
    {
        get
        {
            return mItemImage;
        }
    }

    [Header("아이템의 값(가중치)")]
    [SerializeField] private int mItemValue;
    public int Value
    {
        get
        {
            return mItemValue;
        }
    }

    [Header("월드에 표시할 프리팹")]
    [SerializeField] private GameObject mWorldPrefab;

    public GameObject WorldPrefab
    {
        get
        {
            return mWorldPrefab;
        }
    }
}
