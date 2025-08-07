using System.Collections;
using UnityEngine;

/// <summary>
/// �� ���� ������(�Ǵ� ���� ��ü)�� �ٰ����� �ش� �������� �ݰų�, ��ȣ�ۿ� �� �� �ֵ��� ���ִ� ��ũ��Ʈ
/// �÷��̾��� ������Ʈ�� �ڽ����� ���� EmptyObject�� Trigger Collider�� �߰��Ͽ� ���
/// </summary>
public class ItemInteractor : MonoBehaviour
{

    private bool mIsPickupActive = false;  //������ ������ �����Ѱ�?

    private WorldItem mCurrentItem; //���� ��ȣ�ۿ� ������(�Ⱦ� ������) ItemPickupPoint ����

    [SerializeField] private InventoryMain mInventory; //�κ��丮 ����

    private void Update()
    {
        if (mIsPickupActive) { TryPickItem(); }
    }

    /// <summary>
    /// �������� �ֿ� �� �ִ��� Ȯ���Ѵ�.
    /// </summary>
    private void TryPickItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //�ֿ� �� �ִ� �������̶��?
            if (mCurrentItem.Item.Type > ItemType.NONE)
            {
                //���� �κ��丮 ������ ��������
                InventorySlot[] allitems = mInventory.GetAllItems();

                int count = 0;
                for (; count < allitems.Length; ++count)
                {
                    //���� ������ ĭ�� null�̶�� �ֿ� �� �ִ� ����
                    if (allitems[count].Item == null) {
                        mIsPickupActive = true;
                        break; 
                    }

                    //���� ������ĭ�� null�� �ƴ�����, ���� �����۰� �����ϸ鼭 ��ø�� ������ �������̶�� �ֿ� �� �ִ� ����
                    if (allitems[count].Item.ItemID == mCurrentItem.Item.ItemID && allitems[count].Item.CanOverlap)
                    {
                        mIsPickupActive = true; 
                        break; 
                    }
                }

                //��� ĭ�� null�� �ƴϰ�, ��ø�� �Ұ����ϸ� �ֿ� �� ����
                if (count == allitems.Length)
                {
                    mIsPickupActive = false;
                    return; 
                }

                //������ �ݴ� ȿ���� ���
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

            Debug.LogFormat("������ : {0} ȹ�� ����", mCurrentItem.Item.name);
            

            mIsPickupActive = true;

            return;
        }
        else
        {
            ItemInfoDisappear();
        }
    }

    /// <summary>
    /// ������ ���� �����ֱ⸦ ��Ȱ��ȭ �Ѵ�.
    /// </summary>
    private void ItemInfoDisappear()
    {
        //�Ⱦ� ��Ȱ��ȭ
        mIsPickupActive = false;

        //���� �������� null
        mCurrentItem = null;
    }

    /// <summary>
    /// �������� �����Ѵ�.
    /// </summary>
    private void TryPickUp()
    {
        if (mIsPickupActive)
        {
            // mItemActionCustomFunc.InteractionItem(mCurrentItem.Item, mCurrentItem.gameObject); (�� �ۿ����� ���� X)

            if (mCurrentItem.Item.Type != ItemType.NONE)
            {
                mInventory.AcquireItem(mCurrentItem.Item);
                Destroy(mCurrentItem.gameObject);
            }

            ItemInfoDisappear();
        }
    }
}