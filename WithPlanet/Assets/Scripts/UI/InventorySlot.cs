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


    [Header("해당 슬롯에 어떠한 타입만 들어올 수 있는지 타입 마스크")]
    [SerializeField] private ItemType mSlotMask;

    private int mItemCount; //현재 슬롯에 들어있는 아이템의 개수 


    [Header("아이템 슬롯에 있는 UI 오브젝트")]
    [SerializeField] private Image mItemImage; //아이템의 이미지
    [SerializeField] private TMP_Text mTextCount; //아이템의 개수 텍스트
    [SerializeField] private TMP_Text mName; //아이템의 이름 텍스트 

    void Awake()
    {
        // 자식으로 있는 Image/TMP_Text 컴포넌트를 자동으로 캐싱
        mItemImage = transform.Find("ItemImage").GetComponent<Image>();
        mTextCount = transform.Find("ItemCount").GetComponent<TMP_Text>();
    }
    public bool IsMask(Item item)
    {
        return ((int)item.Type & (int)mSlotMask) == 0 ? false : true;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(Item item, int count = 1)
    {
        mItem = item;
        mItemCount += count;
        mItemImage.sprite = item.Image;
        mName.text = item.Name;
        mTextCount.text = count.ToString();

    }

    // 현재 슬롯의 아이템 개수 업데이트
    public void UpdateSlotCount(int count)
    {
        Debug.Log($"[UpdateSlotCount] 이전={mItemCount}, 더하기={count}");
        mItemCount += count; //테스트 값
        mTextCount.text = mItemCount.ToString();

        if (mItemCount <= 0)
            ClearSlot();
    }

    // 현재 슬롯 비우기
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

