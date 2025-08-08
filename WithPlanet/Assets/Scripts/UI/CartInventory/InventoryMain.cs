using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using TMPro;

public class InventoryMain : InventoryBase
{

    public static bool IsInventoryActive = false; //�κ��丮 Ȱ��ȭ ����
    [SerializeField] private TMP_Text mTotalValue;

    // Start is called before the first frame update
    void Awake() 
    {
        base.Awake(); //base = ��ӹ��� �θ� Ŭ����(InventoryBase)
                      //�ڽ��� awake ȣ���� �� �θ��� awake�� ���� ȣ��ǵ��� �Ѵ�.
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
            // �������� �ִ� ���Ը� ���
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
        //��ø�� �����ϴٸ�?
        if (item.CanOverlap)
        {
            for (int i = 0; i < mSlots.Length; i++)
            {
                // ������ ���� �ʾ����� 
                if (mSlots[i].Item != null && mSlots[i].Item.ItemID == item.ItemID)
                {
                        //���� ������ ������ ����(Count)�� �����Ѵ�.
                    mSlots[i].AddItem(count);
                    return;
                }
            }
        }
        for (int i = 0; i < mSlots.Length; i++)
        {
            //����ũ�� ����Ͽ� �ش� ������ ����ũ�� ���Ǵ� ��ġ�ΰ�쿡�� �������� ����ֵ��� �Ѵ�.
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
            OpenInventory(); // �κ��丮�� ���������� ����
        }
        else
        {
            CloseInventory(); // �κ��丮�� ���������� �ݱ� 
        }
    }

    private void OpenInventory()
    {
        mInventoryBase.SetActive(true); // InventoryBase�� mInventoryBase�� Ȱ��ȭ
        IsInventoryActive = true; // �κ��丮 Ȱ��ȭ ���� ������Ʈ

        Cursor.lockState = CursorLockMode.None; // Ŀ�� Ȱ��ȭ 
        Cursor.visible = true; // ȭ�鿡 Ŀ���� ���̵��� ���� 
    }

    private void CloseInventory()
    {
        mInventoryBase.SetActive(false);
        IsInventoryActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
