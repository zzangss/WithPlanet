using UnityEngine;
using System.Collections;


public class PlayerItemManager : MonoBehaviour
{
    //아이템 관리 
    public float moveToHoldDuration = 1f;
    public Vector3 holdingOffset = new Vector3(6f, 2f, 0.5f); //플레이어 기본 왼쪽 오프셋
    public Vector3 holdOffset = new Vector3(0f, 18f, 0.5f); // 플레이어 앞쪽 위 오프셋
    public float pickupRange = 2f; // 아이템 획득 범위
    public float dropForce = 5f; // 버릴 때 힘
    public PlayerMoveController playerMoveController; // 플레이어 이동 컨트롤러


    public Item currentItem = null; // 현재 플레이어가 들고 있는 아이템  
    public Transform itemHoldPoint; // 아이템을 들고 있을 위치
    public Transform itemHoldingPoint; // 아이템을 들고 있는 위치
    private Animator playerAnimator; // 플레이어 애니메이터


    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerMoveController = GetComponent<PlayerMoveController>(); 
        //아이템을 get할때의 위치 (기본 왼쪽으로 설정)
        GameObject holdingPos = new GameObject("ItemHoldingPosition");
        holdingPos.transform.SetParent(transform);
        holdingPos.transform.localPosition = holdingOffset; 
        itemHoldingPoint = holdingPos.transform;


        // 아이템을 들 위치 생성
        GameObject holdPos = new GameObject("ItemHoldPosition");
        holdPos.transform.SetParent(transform);
        holdPos.transform.localPosition = holdOffset;
        itemHoldPoint = holdPos.transform;
    }

    void Update()
    {
        // 스페이스바로 아이템 획득/버리기
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

    // 주변 아이템 찾아서 획득
    void TryPickupItem()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider col in nearbyObjects)
        {
            Item item = col.GetComponent<Item>();
            if (item != null)
            {
                PickupItem(item);
                break; // 첫번째 아이템만 획득 
            }
        }
    }

    // 아이템 획득
    void PickupItem(Item item)
    {
       

        currentItem = item;

        Vector3 holdingPosition = holdingOffset;
        if (playerMoveController != null && !playerMoveController.isFlipped)
      
        {
            holdingPosition.x = -holdingPosition.x; // 6f → -6f로 변경
        }
        itemHoldingPoint.localPosition = holdingPosition;

        //아이템을 플레이어의 손으로 이동
        item.transform.SetParent(itemHoldingPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        // 물리 비활성화 (Item 스크립트의 Rigidbody 사용)
        Rigidbody itemRb = item.GetRigidbody();
        if (itemRb != null)
        {
            itemRb.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
            itemRb.useGravity = false; // 중력도 끄기
        }

        // 콜라이더 트리거로 변경 (선택사항 - 다른 오브젝트와 충돌 방지)
        Collider itemCollider = item.GetCollider();
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }
        //드는 애니메이션 트리거 설정
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("GetItem");

        }
        StartCoroutine(MoveToHoldPosition(item));

 
        // 아이템 타입에 따른 처리 효과 다르게하기
        if (item.itemType == Item.Type.CoreTreasure)
        {
            Debug.Log("코어 아이템 획득: " + item.itemName);
        }
        else if (item.itemType == Item.Type.Normal)
        {
            Debug.Log("일반 아이템 획득: " + item.itemName);
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

        // 플레이어가 왼쪽(isFlipped == true)인지, 오른쪽인지 확인
        float directionX = playerMoveController != null && playerMoveController.isFlipped ? 1f : -1f;

        // 드롭 위치 및 방향 설정
        Vector3 dropOffset = new Vector3(directionX * 9f, 0.5f, 0f);
        Vector3 dropPosition = transform.position + dropOffset;
        currentItem.transform.position = dropPosition;

        Vector3 dropDirection = new Vector3(directionX, 0.5f, 0f).normalized;

        // 물리 적용
        Rigidbody itemRb = currentItem.GetRigidbody();
        if (itemRb != null)
        {
            itemRb.isKinematic = false;
            itemRb.useGravity = true;
            itemRb.AddForce(dropDirection * dropForce, ForceMode.Impulse);
        }
        // 애니메이션 트리거 설정
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("DropItem");
        }

        // 콜라이더 원래대로 복구
        Collider itemCollider = currentItem.GetCollider();
        if (itemCollider != null)
        {
            itemCollider.isTrigger = false;
        }

        // 애니메이션 업데이트
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("HasItem", false);
        }

        currentItem = null;
    }

   
}