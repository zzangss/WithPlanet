using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMain : InventoryBase
{

    public static bool IsInventoryActive = false; //�κ��丮 Ȱ��ȭ ����

    // Start is called before the first frame update
    void Awake() 
    {
        base.Awake(); //base = ��ӹ��� �θ� Ŭ����(InventoryBase)
                      //�ڽ��� awake ȣ���� �� �θ��� awake�� ���� ȣ��ǵ��� �Ѵ�.
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenInventory();
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
                    targetSlot.UpdateSlotCount(count);
                }
            }
        }
        else
        {
            targetSlot.AddItem(item, count);
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
                    mSlots[i].UpdateSlotCount(count);
                    return;
                }
            }
        }
        for (int i = 0; i < mSlots.Length; i++)
        {
            //����ũ�� ����Ͽ� �ش� ������ ����ũ�� ���Ǵ� ��ġ�ΰ�쿡�� �������� ����ֵ��� �Ѵ�.
            if (mSlots[i].Item == null)
            {
                mSlots[i].AddItem(item, count);
                return;
            }
        }

    }

    private void TryOpenInventory()
    {

        if (Input.GetKeyDown(KeyCode.I)) // I Ű�� ������ ��
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
