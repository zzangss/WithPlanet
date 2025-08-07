using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ItemType  // ������ ����
{
    /// <summary>
    /// NONE Type�� �������� �����ϱ����� EŰ�� �������, �κ��丮�� ������ �ʴ´�.
    /// Ư���� ��ȣ�ۿ��� �ִ� ������Ʈ�� ����Ѵ�.
    /// </summary>
    NONE = 0b0, //0
    SKILL = 0b1, //1

    //��� ������ ����
    //��� ������ Ÿ�Կ��� �߰��Ǵ°��, �����ϴ� ������ �߰��Ѵ�.
    Equipment_HELMET = 0b10, //2
    Equipment_ARMORPLATE = 0b100, //4
    Equipment_GLOVE = 0b1000, //8
    Equipment_PANTS = 0b10000, //16
    Equipment_SHOES = 0b100000, //32

    //��� �������� �ƴ� �����۵�(�Ҹ�, ��Ÿ, ���, ����Ʈ������ ���)
    Etc = 0b1000000, //64
    Consumable = 0b10000000, //128
    Ingredient = 0b100000000, //256
    Quest = 0b1000000000, //512
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

    [Header("�κ��丮���� ������ �������� �̹���")]
    [SerializeField] private Sprite mItemImage;
    public Sprite Image
    {
        get
        {
            return mItemImage;
        }
    }
}
