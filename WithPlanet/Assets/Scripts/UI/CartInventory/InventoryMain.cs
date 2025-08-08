using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using TMPro;

public class InventoryMain : InventoryBase
{

    public static bool IsInventoryActive = false; //인벤토리 활성화 여부
    [SerializeField] private TMP_Text mTotalValue;

    // Start is called before the first frame update
    void Awake() 
    {
        base.Awake(); //base = 상속받은 부모 클래스(InventoryBase)
                      //자식이 awake 호출할 때 부모의 awake도 같이 호출되도록 한다.
    }

    // Update is called once per frame
    void Update()
    {
        mTotalValue.text = CalTotalItemValue().ToString();
    }

    private int CalTotalItemValue()
    {
        int total = 0;
        foreach (var slot in mSlots)
        {
            // 아이템이 있는 슬롯만 계산
            if (slot.Item != null)
            {
                total += slot.GetItemCount() * slot.Item.Value;
            }
        }

        return total;
    }

    public bool GetIsInventoryActive()
    {
        return IsInventoryActive;
    }

    public InventorySlot[] GetAllItems()
    {
        return mSlots;
    }

    public void AcquireItem(Item item, InventorySlot targetSlot, int count = 1)
    {
        if (item.CanOverlap)
        {
            if (targetSlot.Item != null && targetSlot.IsMask(item))
            {
                if (targetSlot.Item.ItemID == item.ItemID)
                {
                    targetSlot.AddItem(count);
                }
            }
        }
        else
        {
            targetSlot.SetItem(item, count);
        }
    }

    public void AcquireItem(Item item, int count = 1)
    {
        //중첩이 가능하다면?
        if (item.CanOverlap)
        {
            for (int i = 0; i < mSlots.Length; i++)
            {
                // 슬롯이 비지 않았으면 
                if (mSlots[i].Item != null && mSlots[i].Item.ItemID == item.ItemID)
                {
                        //현재 슬롯의 아이템 개수(Count)를 갱신한다.
                    mSlots[i].AddItem(count);
                    return;
                }
            }
        }
        for (int i = 0; i < mSlots.Length; i++)
        {
            //마스크를 사용하여 해당 슬롯이 마스크에 허용되는 위치인경우에만 아이템을 집어넣도록 한다.
            if (mSlots[i].Item == null)
            {
                mSlots[i].SetItem(item, count);
                return;
            }
        }

    }

    public void TryOpenCloseInventory()
    {

        if (!IsInventoryActive) 
        {
            OpenInventory(); // 인벤토리가 닫혀있으면 열기
        }
        else
        {
            CloseInventory(); // 인벤토리가 열려있으면 닫기 
        }
    }

    private void OpenInventory()
    {
        mInventoryBase.SetActive(true); // InventoryBase의 mInventoryBase를 활성화
        IsInventoryActive = true; // 인벤토리 활성화 여부 업데이트

        Cursor.lockState = CursorLockMode.None; // 커서 활성화 
        Cursor.visible = true; // 화면에 커서가 보이도록 설정 
    }

    private void CloseInventory()
    {
        mInventoryBase.SetActive(false);
        IsInventoryActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
