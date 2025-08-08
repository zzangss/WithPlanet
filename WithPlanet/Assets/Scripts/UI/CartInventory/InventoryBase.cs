using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 
// �߻� Ŭ������ ����
abstract public class InventoryBase : MonoBehaviour
{
    
    [SerializeField] protected GameObject mInventoryBase;
    [SerializeField] protected GameObject mInventorySlotsParent;
    protected InventorySlot[] mSlots;

    protected void Awake()
    {
        if (mInventoryBase.activeSelf) // �κ��丮�� Ȱ��/��Ȱ�� ���� Ȯ��. 
        {
            mInventoryBase.SetActive(false); // �κ��丮�� Ȱ��ȭ �Ǿ������� ��Ȱ������ �ٲ�.
        }

        mSlots = mInventorySlotsParent.GetComponentsInChildren<InventorySlot>(); // �κ��丮 ������ �迭�� ���� 
    }

}
