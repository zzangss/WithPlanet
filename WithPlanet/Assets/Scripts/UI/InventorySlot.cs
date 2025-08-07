using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler,
                            IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Item mItem;
    public Item Item
    {
        get
        {
            return mItem;
        }
    }


    [Header("�ش� ���Կ� ��� Ÿ�Ը� ���� �� �ִ��� Ÿ�� ����ũ")]
    [SerializeField] private ItemType mSlotMask;

    private int mItemCount; //���� ���Կ� ����ִ� �������� ���� 


    [Header("������ ���Կ� �ִ� UI ������Ʈ")]
    [SerializeField] private Image mItemImage; //�������� �̹���
    [SerializeField] private TMP_Text mTextCount; //�������� ���� �ؽ�Ʈ
    [SerializeField] private TMP_Text mName; //�������� �̸� �ؽ�Ʈ 

    void Awake()
    {
        // �ڽ����� �ִ� Image/TMP_Text ������Ʈ�� �ڵ����� ĳ��
        mItemImage = transform.Find("ItemImage").GetComponent<Image>();
        mTextCount = transform.Find("ItemCount").GetComponent<TMP_Text>();
    }
    public bool IsMask(Item item)
    {
        return ((int)item.Type & (int)mSlotMask) == 0 ? false : true;
    }

    // �κ��丮�� ���ο� ������ ���� �߰�
    public void AddItem(Item item, int count = 1)
    {
        mItem = item;
        mItemCount += count;
        mItemImage.sprite = item.Image;
        mName.text = item.Name;
        mTextCount.text = count.ToString();

    }

    // ���� ������ ������ ���� ������Ʈ
    public void UpdateSlotCount(int count)
    {
        Debug.Log($"[UpdateSlotCount] ����={mItemCount}, ���ϱ�={count}");
        mItemCount += count; //�׽�Ʈ ��
        mTextCount.text = mItemCount.ToString();

        if (mItemCount <= 0)
            ClearSlot();
    }

    // ���� ���� ����
    public void ClearSlot()
    {
        mItem = null;
        mItemCount = 0;
        mItemImage = null;
        mName = null;

        mTextCount.text = "";
    }

    pubilc void OnBeginDrag(PointerEventData eventData)
    {
        if(mItem != null)
        {
            
        }
    }
}

