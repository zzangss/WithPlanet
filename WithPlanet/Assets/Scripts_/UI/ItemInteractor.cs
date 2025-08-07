using System.Collections;
using UnityEngine;

/// <summary>
/// 씬 내의 아이템(또는 정적 물체)에 다가가면 해당 아이템을 줍거나, 상호작용 할 수 있도록 해주는 스크립트
/// 플레이어의 오브젝트에 자식으로 넣은 EmptyObject에 Trigger Collider을 추가하여 사용
/// </summary>
public class ItemInteractor : MonoBehaviour
{

    private bool mIsPickupActive = false;  //아이템 습득이 가능한가?

    private WorldItem mCurrentItem; //현재 상호작용 가능한(픽업 가능한) ItemPickupPoint 참조

    [SerializeField] private InventoryMain mInventory; //인벤토리 메인

    private void Update()
    {
        if (mIsPickupActive) { TryPickItem(); }
    }

    /// <summary>
    /// 아이템을 주울 수 있는지 확인한다.
    /// </summary>
    private void TryPickItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //주울 수 있는 아이템이라면?
            if (mCurrentItem.Item.Type > ItemType.NONE)
            {
                //현재 인벤토리 아이템 가져오기
                InventorySlot[] allitems = mInventory.GetAllItems();

                int count = 0;
                for (; count < allitems.Length; ++count)
                {
                    //현재 아이템 칸이 null이라면 주울 수 있는 상태
                    if (allitems[count].Item == null) {
                        mIsPickupActive = true;
                        break; 
                    }

                    //현재 아이템칸이 null이 아니지만, 현재 아이템과 동일하면서 중첩이 가능한 아이템이라면 주울 수 있는 상태
                    if (allitems[count].Item.ItemID == mCurrentItem.Item.ItemID && allitems[count].Item.CanOverlap)
                    {
                        mIsPickupActive = true; 
                        break; 
                    }
                }

                //모든 칸이 null이 아니고, 중첩이 불가능하면 주울 수 없음
                if (count == allitems.Length)
                {
                    mIsPickupActive = false;
                    return; 
                }

                //아이템 줍는 효과음 재생
                // SoundManager.Instance.PlaySound2D("GrabItem " + SoundManager.Range(1, 3));
            }

            TryPickUp();
            ItemInfoDisappear();
        }
    } 

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Item")
        {
            WorldItem collidedItem = other.transform.GetComponent<WorldItem>();

            if(mCurrentItem == collidedItem)
            {
                return;
            }

            mCurrentItem = collidedItem;

            Debug.LogFormat("아이템 : {0} 획득 가능", mCurrentItem.Item.name);
            

            mIsPickupActive = true;

            return;
        }
        else
        {
            ItemInfoDisappear();
        }
    }

    /// <summary>
    /// 아이템 정보 보여주기를 비활성화 한다.
    /// </summary>
    private void ItemInfoDisappear()
    {
        //픽업 비활성화
        mIsPickupActive = false;

        //현재 아이템은 null
        mCurrentItem = null;
    }

    /// <summary>
    /// 아이템을 습득한다.
    /// </summary>
    private void TryPickUp()
    {
        if (mIsPickupActive)
        {
            // mItemActionCustomFunc.InteractionItem(mCurrentItem.Item, mCurrentItem.gameObject); (이 글에서는 설명 X)

            if (mCurrentItem.Item.Type != ItemType.NONE)
            {
                mInventory.AcquireItem(mCurrentItem.Item);
                Destroy(mCurrentItem.gameObject);
            }

            ItemInfoDisappear();
        }
    }
}