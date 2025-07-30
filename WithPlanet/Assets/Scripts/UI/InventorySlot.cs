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


    [Header("해당 슬롯에 어떠한 타입만 들어올 수 있는지 타입 마스크")]
    [SerializeField] private ItemType mSlotMask;

    private int mItemCount; //현재 슬롯에 들어있는 아이템의 개수 


    [Header("아이템 슬롯에 있는 UI 오브젝트")]
    [SerializeField] private Image mItemImage; //아이템의 이미지
    [SerializeField] private TMP_Text mTextCount; //아이템의 개수 텍스트
    [SerializeField] private Image mBackImage; //아이템의 이름 텍스트 

    [SerializeField] private Image mValueImage;
    [SerializeField] private TMP_Text mVauleCount;


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
    public void SetItem(Item item, int count = 1)
    {
        mItem = item;
        mItemCount += count;
        mItemImage.sprite = item.Image;
        mTextCount.text = count.ToString();

    }

    // 현재 슬롯의 아이템 개수 업데이트
    public void AddItem(int count)
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
        mItemImage.sprite = null;
        mTextCount.text = "";
    }

    // 아이템 드래그 시작 
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
            Debug.LogWarning("드래그 슬롯 or 현재 슬롯이 널");
            return;
        }
        if(DragSlot.Instance.CurrentSlot.Item == null)
        {
            Debug.LogWarning("드래그된 아이템은 NULL");
            return;
        }


        if (DragSlot.Instance.IsShiftMode && mItem != null) { return; }

        //드래그 슬롯에 놓여진 아이템과, 바꿔질 아이템의 마스크가 모두 통과되면 바꾼다.
        if (!IsMask(DragSlot.Instance.CurrentSlot.Item)) { return; }

        //타겟 드래그 슬롯에 이미 아이템이 있는경우, 해당 아이템이 직전의 아이템 슬롯에서 마스크를 체크한다.
        if (mItem != null && !DragSlot.Instance.CurrentSlot.IsMask(mItem)) { return; }

        ChangeSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"{this.name}에마우스 올라옴");

        if (mItem != null)
        {
            ShowItemVaule(mItem);
        }
    }

    // 마우스가 슬롯에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"{this.name}에서 빠져나옴");
    }

    private void ShowItemVaule(Item item)
    {

    }


    private void ChangeSlot()
    {
        if (DragSlot.Instance.CurrentSlot.Item.Type >= ItemType.Etc)
        {
            //쉬프트 모드 관계 없이 일어날 수 있는 경우
            //새로운 슬롯과 이전 슬롯의 아이템ID가 같은경우 개수를 합친다.
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

            //쉬프트 모드인경우 개수를 반으로 나눈다.
            if (DragSlot.Instance.IsShiftMode)
            {
                //changedSlotCount 개수만큼 더하고 뺄것이다 (+와 -의 차이가 0이어야 아이템이 복사, 유실되지 않는다.)
                int changedSlotCount = (int)(DragSlot.Instance.CurrentSlot.mItemCount * 0.5f);

                //(int)로의 형변환으로 인해 0이 되는 경우는 (int)(1 * 0.5f)이기에 단순히 새로운 슬롯으로 옮긴다.
                if (changedSlotCount == 0)
                {
                    SetItem(DragSlot.Instance.CurrentSlot.Item, 1);
                    DragSlot.Instance.CurrentSlot.ClearSlot();
                    return;
                }

                //위 모든 경우가 아닌경우 새로운 슬롯에 새로운 아이템을 생성한다.
                SetItem(DragSlot.Instance.CurrentSlot.Item, changedSlotCount);
                DragSlot.Instance.CurrentSlot.AddItem(-changedSlotCount);
                return;
            }
        }

        //슬롯 서로 교환하기
        Item tempItem = mItem;
        int tempItemCount = mItemCount;

        SetItem(DragSlot.Instance.CurrentSlot.mItem, DragSlot.Instance.CurrentSlot.mItemCount);
        

        if (tempItem != null) { DragSlot.Instance.CurrentSlot.SetItem(tempItem, tempItemCount); }
        else { DragSlot.Instance.CurrentSlot.ClearSlot(); }
    }
}

