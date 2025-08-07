using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, 
    IPointerEnterHandler, IPointerExitHandler

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
    [SerializeField] private Image mBackImage; //�������� �̸� �ؽ�Ʈ 

    [SerializeField] private Image mValueImage;
    [SerializeField] private TMP_Text mVauleCount;


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
    public void SetItem(Item item, int count = 1)
    {
        mItem = item;
        mItemCount += count;
        mItemImage.sprite = item.Image;
        mTextCount.text = count.ToString();

    }

    // ���� ������ ������ ���� ������Ʈ
    public void AddItem(int count)
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
        mItemImage.sprite = null;
        mTextCount.text = "";
    }

    // ������ �巡�� ���� 
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(mItem != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                DragSlot.Instance.IsShiftMode = true;
            }
            else
            {
                DragSlot.Instance.IsShiftMode = false;
            }

            DragSlot.Instance.CurrentSlot = this;
            DragSlot.Instance.DragSetImage(mItemImage);
            DragSlot.Instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mItem != null) {
            DragSlot.Instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.Instance.SetColor(0);
        //DragSlot.Instance.CurrentSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.Instance == null || DragSlot.Instance.CurrentSlot == null)
        {
            Debug.LogWarning("�巡�� ���� or ���� ������ ��");
            return;
        }
        if(DragSlot.Instance.CurrentSlot.Item == null)
        {
            Debug.LogWarning("�巡�׵� �������� NULL");
            return;
        }


        if (DragSlot.Instance.IsShiftMode && mItem != null) { return; }

        //�巡�� ���Կ� ������ �����۰�, �ٲ��� �������� ����ũ�� ��� ����Ǹ� �ٲ۴�.
        if (!IsMask(DragSlot.Instance.CurrentSlot.Item)) { return; }

        //Ÿ�� �巡�� ���Կ� �̹� �������� �ִ°��, �ش� �������� ������ ������ ���Կ��� ����ũ�� üũ�Ѵ�.
        if (mItem != null && !DragSlot.Instance.CurrentSlot.IsMask(mItem)) { return; }

        ChangeSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"{this.name}�����콺 �ö��");

        if (mItem != null)
        {
            ShowItemVaule(mItem);
        }
    }

    // ���콺�� ���Կ��� ����� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"{this.name}���� ��������");
    }

    private void ShowItemVaule(Item item)
    {

    }


    private void ChangeSlot()
    {
        if (DragSlot.Instance.CurrentSlot.Item.Type >= ItemType.Etc)
        {
            //����Ʈ ��� ���� ���� �Ͼ �� �ִ� ���
            //���ο� ���԰� ���� ������ ������ID�� ������� ������ ��ģ��.
            if (mItem != null && mItem.ItemID == DragSlot.Instance.CurrentSlot.Item.ItemID)
            {
                int changedSlotCount;
                if (DragSlot.Instance.IsShiftMode) 
                { 
                    changedSlotCount = (int)(DragSlot.Instance.CurrentSlot.mItemCount * 0.5f); 
                }
                else 
                { 
                    changedSlotCount = DragSlot.Instance.CurrentSlot.mItemCount; 
                }

                AddItem(changedSlotCount);
                DragSlot.Instance.CurrentSlot.AddItem(-changedSlotCount);
                return;
            }

            //����Ʈ ����ΰ�� ������ ������ ������.
            if (DragSlot.Instance.IsShiftMode)
            {
                //changedSlotCount ������ŭ ���ϰ� �����̴� (+�� -�� ���̰� 0�̾�� �������� ����, ���ǵ��� �ʴ´�.)
                int changedSlotCount = (int)(DragSlot.Instance.CurrentSlot.mItemCount * 0.5f);

                //(int)���� ����ȯ���� ���� 0�� �Ǵ� ���� (int)(1 * 0.5f)�̱⿡ �ܼ��� ���ο� �������� �ű��.
                if (changedSlotCount == 0)
                {
                    SetItem(DragSlot.Instance.CurrentSlot.Item, 1);
                    DragSlot.Instance.CurrentSlot.ClearSlot();
                    return;
                }

                //�� ��� ��찡 �ƴѰ�� ���ο� ���Կ� ���ο� �������� �����Ѵ�.
                SetItem(DragSlot.Instance.CurrentSlot.Item, changedSlotCount);
                DragSlot.Instance.CurrentSlot.AddItem(-changedSlotCount);
                return;
            }
        }

        //���� ���� ��ȯ�ϱ�
        Item tempItem = mItem;
        int tempItemCount = mItemCount;

        SetItem(DragSlot.Instance.CurrentSlot.mItem, DragSlot.Instance.CurrentSlot.mItemCount);
        

        if (tempItem != null) { DragSlot.Instance.CurrentSlot.SetItem(tempItem, tempItemCount); }
        else { DragSlot.Instance.CurrentSlot.ClearSlot(); }
    }
}

