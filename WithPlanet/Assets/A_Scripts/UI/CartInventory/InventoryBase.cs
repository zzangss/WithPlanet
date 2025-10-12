using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 
// 추상 클래스로 구현
abstract public class InventoryBase : MonoBehaviour
{
    
    [SerializeField] protected GameObject mInventoryBase;
    [SerializeField] protected GameObject mInventorySlotsParent;
    protected InventorySlot[] mSlots;

    protected void Awake()
    {
        if (mInventoryBase.activeSelf) // 인벤토리의 활성/비활성 여부 확인. 
        {
            mInventoryBase.SetActive(false); // 인벤토리가 활성화 되어있으면 비활성으로 바꿈.
        }

        mSlots = mInventorySlotsParent.GetComponentsInChildren<InventorySlot>(); // 인벤토리 슬롯을 배열로 저장 
    }

}
