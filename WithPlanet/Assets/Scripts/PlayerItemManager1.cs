using UnityEngine;
using System.Collections;


public class PlayerItemManager : MonoBehaviour
{
    //������ ���� 
    public float moveToHoldDuration = 1f;
    public Vector3 holdingOffset = new Vector3(6f, 2f, 0.5f); //�÷��̾� �⺻ ���� ������
    public Vector3 holdOffset = new Vector3(0f, 18f, 0.5f); // �÷��̾� ���� �� ������
    public float pickupRange = 2f; // ������ ȹ�� ����
    public float dropForce = 5f; // ���� �� ��
    public PlayerMoveController playerMoveController; // �÷��̾� �̵� ��Ʈ�ѷ�


    public Item currentItem = null; // ���� �÷��̾ ��� �ִ� ������  
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
                DropItem();
            }
        }
    }

    // �ֺ� ������ ã�Ƽ� ȹ��
    void TryPickupItem()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider col in nearbyObjects)
        {
            Item item = col.GetComponent<Item>();
            if (item != null)
            {
                PickupItem(item);
                break; // ù��° �����۸� ȹ�� 
            }
        }
    }

    // ������ ȹ��
    void PickupItem(Item item)
    {
       

        currentItem = item;

        Vector3 holdingPosition = holdingOffset;
        if (playerMoveController != null && !playerMoveController.isFlipped)
      
        {
            holdingPosition.x = -holdingPosition.x; // 6f �� -6f�� ����
        }
        itemHoldingPoint.localPosition = holdingPosition;

        //�������� �÷��̾��� ������ �̵�
        item.transform.SetParent(itemHoldingPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        // ���� ��Ȱ��ȭ (Item ��ũ��Ʈ�� Rigidbody ���)
        Rigidbody itemRb = item.GetRigidbody();
        if (itemRb != null)
        {
            itemRb.isKinematic = true; // ���� ������ ������ ���� �ʵ��� ����
            itemRb.useGravity = false; // �߷µ� ����
        }

        // �ݶ��̴� Ʈ���ŷ� ���� (���û��� - �ٸ� ������Ʈ�� �浹 ����)
        Collider itemCollider = item.GetCollider();
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }
        //��� �ִϸ��̼� Ʈ���� ����
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("GetItem");

        }
        StartCoroutine(MoveToHoldPosition(item));

 
        // ������ Ÿ�Կ� ���� ó�� ȿ�� �ٸ����ϱ�
        if (item.itemType == Item.Type.CoreTreasure)
        {
            Debug.Log("�ھ� ������ ȹ��: " + item.itemName);
        }
        else if (item.itemType == Item.Type.Normal)
        {
            Debug.Log("�Ϲ� ������ ȹ��: " + item.itemName);
        }
    }
    IEnumerator MoveToHoldPosition(Item item)
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