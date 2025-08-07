using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Flags]
public enum ItemType  // ������ ����
{
    /// <summary>
    /// NONE Type�� �������� �����ϱ����� EŰ�� �������, �κ��丮�� ������ �ʴ´�.
    /// Ư���� ��ȣ�ۿ��� �ִ� ������Ʈ�� ����Ѵ�.
    /// </summary>
    NONE = 0b0, //0

    //�Ҹ�, ��Ÿ, ���, ����Ʈ������ ���
    SKILL = 0b1, //1
    Etc = 0b10, //2
    Consumable = 0b100, //4
    Ingredient = 0b1000, //8
    Quest = 0b10000, //16
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

    [Header("���(��ȣ�ۿ�)�� ������ �������ΰ�?")]
    [SerializeField] private bool mIsInteractivity;
    public bool IsInteractivity
    {
        get
        {
            return mIsInteractivity;
        }
    }

    [Header("�������� ����ϸ� ������°�?")]
    [SerializeField] private bool mIsConsumable;
    public bool IsConsumable
    {
        get
        {
            return mIsConsumable;
        }
    }

    [Header("�������� ���� ��Ÿ��")]
    [SerializeField] private float mItemCooltime = -1;
    public float Cooltime
    {
        get
        {
            return mItemCooltime;
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
