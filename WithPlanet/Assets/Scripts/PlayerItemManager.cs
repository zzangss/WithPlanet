using UnityEngine;
using System.Collections;


public class PlayerItemManager : MonoBehaviour
{
    //������ ���� 
    public float moveToHoldDuration = 1f;
    public Vector3 holdingOffset = new Vector3(6f, 2f, 0.5f); //�÷��̾� �⺻ ���� ������
    public Vector3 holdOffset = new Vector3(0f, 18f, 0.5f); // �÷��̾� ���� �� ������
    public float pickupRange = 8f; // ������ ȹ�� ����
    public float dropForce = 5f; // ���� �� ��
    public PlayerMoveController playerMoveController; // �÷��̾� �̵� ��Ʈ�ѷ�
    public InventoryMain inventoryMain; // īƮ �κ��丮 

    public WorldItem currentItem = null; // ���� �÷��̾ ��� �ִ� ������  
    public Transform itemHoldPoint; // �������� ��� ���� ��ġ
    public Transform itemHoldingPoint; // �������� ��� �ִ� ��ġ
    private Animator playerAnimator; // �÷��̾� �ִϸ�����

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerMoveController = GetComponent<PlayerMoveController>(); 
        //�������� get�Ҷ��� ��ġ (�⺻ �������� ����)
        GameObject holdingPos = new GameObject("ItemHoldingPosition");
        holdingPos.transform.SetParent(transform);
        holdingPos.transform.localPosition = holdingOffset; 
        itemHoldingPoint = holdingPos.transform;


        // �������� �� ��ġ ����
        GameObject holdPos = new GameObject("ItemHoldPosition");
        holdPos.transform.SetParent(transform);
        holdPos.transform.localPosition = holdOffset;
        itemHoldPoint = holdPos.transform;
    }

    void Update()
    {
        // �����̽��ٷ� ������ ȹ��/������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentItem == null)
            {
                TryPickupItem();
            }
            else
            {
                Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, pickupRange);

                int i;
                for(i = 0; i<nearbyObjects.Length; i++)
                {

                    if (nearbyObjects[i].CompareTag("Cart"))
                    {
                        Debug.Log("������ īƮ�� �ֱ�");
                        TryPutItem();
                    }

                }
                if (i >= nearbyObjects.Length)
                {
                    DropItem();
                }
            }
        }
    }

    private void TryPutItem()
    {
        //���� �κ��丮 ������ ��������
        InventorySlot[] allitems = inventoryMain.GetAllItems();

        bool isPutable = false;
        int count = 0;
        for (; count < allitems.Length; ++count)
        {
            //���� ������ ĭ�� null�̶�� �ֿ� �� �ִ� ����
            if (allitems[count].Item == null)
            {
                isPutable = true;
                break;
            }

            //���� ������ĭ�� null�� �ƴ�����, ���� �����۰� �����ϸ鼭 ��ø�� ������ �������̶�� �ֿ� �� �ִ� ����
            if (allitems[count].Item.ItemID == currentItem.Item.ItemID && allitems[count].Item.CanOverlap)
            {
                isPutable = true;
                break;
            }
        }

        //��� ĭ�� null�� �ƴϰ�, ��ø�� �Ұ����ϸ� �ֿ� �� ����
        if (count == allitems.Length)
        {
            isPutable = false;
            return;
        }

       
        // ������ ���
        if (isPutable)
        {
            inventoryMain.AcquireItem(currentItem.Item);

            //������ �ݴ� ȿ���� ���
            // SoundManager.Instance.PlaySound2D("GrabItem " + SoundManager.Range(1, 3));

            Destroy(currentItem.gameObject);

            StopAllCoroutines();

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("DropItem");
            }

            if (playerAnimator != null)
            {
                playerAnimator.SetBool("HasItem", false);
            }


            currentItem = null;

        }


    }
  

    // �ֺ� ������ ã�Ƽ� ȹ��
    private void TryPickupItem()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider col in nearbyObjects)
        {

            if (!col.CompareTag("Item")) continue;

            // �θ� �ڽĿ��� �پ����� �� ������ �̷��� �˻�
            WorldItem item = col.GetComponent<WorldItem>() ??
                               col.GetComponentInParent<WorldItem>() ??
                               col.GetComponentInChildren<WorldItem>();

            if (item != null)
            {
                Debug.Log("������ �ݱ�");
                PickupItem(item);
                break;
            }

            if (item == null)
            {
                Debug.Log("������ ����");
            }
        }
    }

    // ������ ȹ��
    void PickupItem(WorldItem worldItem)
    {
        currentItem = worldItem;

        Vector3 holdingPosition = holdingOffset;
        if (playerMoveController != null && !playerMoveController.isFlipped)
      
        {
            holdingPosition.x = -holdingPosition.x; // 6f �� -6f�� ����
        }
        itemHoldingPoint.localPosition = holdingPosition;

        //�������� �÷��̾��� ������ �̵�
        worldItem.transform.SetParent(itemHoldingPoint);
        worldItem.transform.localPosition = Vector3.zero;
        worldItem.transform.localRotation = Quaternion.identity;

        // ���� ��Ȱ��ȭ (Item ��ũ��Ʈ�� Rigidbody ���)
        Rigidbody itemRb = worldItem.GetRigidbody();
        if (itemRb != null)
        {
            itemRb.isKinematic = true; // ���� ������ ������ ���� �ʵ��� ����
            itemRb.useGravity = false; // �߷µ� ����
        }

        // �ݶ��̴� Ʈ���ŷ� ���� (���û��� - �ٸ� ������Ʈ�� �浹 ����)
        Collider itemCollider = worldItem.GetCollider();
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }
        //��� �ִϸ��̼� Ʈ���� ����
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("GetItem");

        }
        StartCoroutine(MoveToHoldPosition(worldItem));

        // ������ Ÿ�Կ� ���� ó�� ȿ�� �ٸ����ϱ�
        if (worldItem.Item.Type == ItemType.CORE)
        {
            Debug.Log("�ھ� ������ ȹ��: " + worldItem.Item.Name);
        }
        else if (worldItem.Item.Type == ItemType.NORMAL)
        {
            Debug.Log("�Ϲ� ������ ȹ��: " + worldItem.Item.Name);
        }
    }
    IEnumerator MoveToHoldPosition(WorldItem item)
    {
        yield return new WaitForSeconds(0.5f);

        item.transform.SetParent(itemHoldPoint);

        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        Vector3 targetPos = Vector3.zero;
        Quaternion targetRot = Quaternion.identity;

        float elapsedTime = 0f;
        while (elapsedTime < moveToHoldDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveToHoldDuration;

            item.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            item.transform.localRotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        item.transform.localPosition = targetPos;
        item.transform.localRotation = targetRot;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("HasItem", true);
        }
    }

    void DropItem()
    {
        if (currentItem == null) return;

        StopAllCoroutines();

        currentItem.transform.SetParent(null);

        // �÷��̾ ����(isFlipped == true)����, ���������� Ȯ��
        float directionX = playerMoveController != null && playerMoveController.isFlipped ? 1f : -1f;

        // ��� ��ġ �� ���� ����
        Vector3 dropOffset = new Vector3(directionX * 9f, 0.5f, 0f);
        Vector3 dropPosition = transform.position + dropOffset;
        currentItem.transform.position = dropPosition;

        Vector3 dropDirection = new Vector3(directionX, 0.5f, 0f).normalized;

        // ���� ����
        Rigidbody itemRb = currentItem.GetRigidbody();
        if (itemRb != null)
        {
            itemRb.isKinematic = false;
            itemRb.useGravity = true;
            itemRb.AddForce(dropDirection * dropForce, ForceMode.Impulse);
        }
        // �ִϸ��̼� Ʈ���� ����
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("DropItem");
        }

        // �ݶ��̴� ������� ����
        Collider itemCollider = currentItem.GetCollider();
        if (itemCollider != null)
        {
            itemCollider.isTrigger = false;
        }

        // �ִϸ��̼� ������Ʈ
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("HasItem", false);
        }

        currentItem = null;
    }

   
}