using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Flags]
public enum ItemType  // ������ ����
{
    NONE = 0b0,
    NORMAL = 0b1, //1
    CORE = 0b10 //2
}

[CreateAssetMenu(fileName = "Item", menuName =  "Add Item/Item")]
public class Item : ScriptableObject
{
    [Header("������ �������� ID(�ߺ��Ұ�)")]
    [SerializeField] private int mItemID;
    public int ItemID
    {
        get
        {
            return mItemID;
        }
    }

    [Header("�������� ��ø�� �����Ѱ�?")]
    [SerializeField] private bool mCanOverlap;
    public bool CanOverlap
    {
        get
        {
            return mCanOverlap;
        }
    }

    [Header("�������� Ÿ��")]
    [SerializeField] private ItemType mItemType;

    public ItemType Type
    {
        get
        {
            return mItemType;
        }
    }

    [Header("�������� �̸�")]
    [SerializeField] private string mName;

    public string Name
    {
        get
        {
            return mName;
        }
    }

    [Header("�������� �̹���")]
    [SerializeField] private Sprite mItemImage;
    public Sprite Image
    {
        get
        {
            return mItemImage;
        }
    }

    [Header("�������� ��(����ġ)")]
    [SerializeField] private int mItemValue;
    public int Value
    {
        get
        {
            return mItemValue;
        }
    }

    [Header("���忡 ǥ���� ������")]
    [SerializeField] private GameObject mWorldPrefab;

    public GameObject WorldPrefab
    {
        get
        {
            return mWorldPrefab;
        }
    }
}
