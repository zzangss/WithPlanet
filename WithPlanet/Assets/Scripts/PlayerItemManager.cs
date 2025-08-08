using UnityEngine;
using System.Collections;


public class PlayerItemManager : MonoBehaviour
{
    //아이템 관리 
    public float moveToHoldDuration = 1f;
    public Vector3 holdingOffset = new Vector3(6f, 2f, 0.5f); //플레이어 기본 왼쪽 오프셋
    public Vector3 holdOffset = new Vector3(0f, 18f, 0.5f); // 플레이어 앞쪽 위 오프셋
    public float pickupRange = 8f; // 아이템 획득 범위
    public float dropForce = 5f; // 버릴 때 힘
    public PlayerMoveController playerMoveController; // 플레이어 이동 컨트롤러
    public InventoryMain inventoryMain; // 카트 인벤토리 

    public WorldItem currentItem = null; // 현재 플레이어가 들고 있는 아이템  
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
                Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, pickupRange);

                int i;
                for(i = 0; i<nearbyObjects.Length; i++)
                {

                    if (nearbyObjects[i].CompareTag("Cart"))
                    {
                        Debug.Log("아이템 카트에 넣기");
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
        //현재 인벤토리 아이템 가져오기
        InventorySlot[] allitems = inventoryMain.GetAllItems();

        bool isPutable = false;
        int count = 0;
        for (; count < allitems.Length; ++count)
        {
            //현재 아이템 칸이 null이라면 주울 수 있는 상태
            if (allitems[count].Item == null)
            {
                isPutable = true;
                break;
            }

            //현재 아이템칸이 null이 아니지만, 현재 아이템과 동일하면서 중첩이 가능한 아이템이라면 주울 수 있는 상태
            if (allitems[count].Item.ItemID == currentItem.Item.ItemID && allitems[count].Item.CanOverlap)
            {
                isPutable = true;
                break;
            }
        }

        //모든 칸이 null이 아니고, 중첩이 불가능하면 주울 수 없음
        if (count == allitems.Length)
        {
            isPutable = false;
            return;
        }

       
        // 아이템 얻기
        if (isPutable)
        {
            inventoryMain.AcquireItem(currentItem.Item);

            //아이템 줍는 효과음 재생
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
  

    // 주변 아이템 찾아서 획득
    private void TryPickupItem()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider col in nearbyObjects)
        {

            if (!col.CompareTag("Item")) continue;

            // 부모나 자식에도 붙어있을 수 있으니 이렇게 검색
            WorldItem item = col.GetComponent<WorldItem>() ??
                               col.GetComponentInParent<WorldItem>() ??
                               col.GetComponentInChildren<WorldItem>();

            if (item != null)
            {
                Debug.Log("아이템 줍기");
                PickupItem(item);
                break;
            }

            if (item == null)
            {
                Debug.Log("아이템 없음");
            }
        }
    }

    // 아이템 획득
    void PickupItem(WorldItem worldItem)
    {
        currentItem = worldItem;

        Vector3 holdingPosition = holdingOffset;
        if (playerMoveController != null && !playerMoveController.isFlipped)
      
        {
            holdingPosition.x = -holdingPosition.x; // 6f → -6f로 변경
        }
        itemHoldingPoint.localPosition = holdingPosition;

        //아이템을 플레이어의 손으로 이동
        worldItem.transform.SetParent(itemHoldingPoint);
        worldItem.transform.localPosition = Vector3.zero;
        worldItem.transform.localRotation = Quaternion.identity;

        // 물리 비활성화 (Item 스크립트의 Rigidbody 사용)
        Rigidbody itemRb = worldItem.GetRigidbody();
        if (itemRb != null)
        {
            itemRb.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
            itemRb.useGravity = false; // 중력도 끄기
        }

        // 콜라이더 트리거로 변경 (선택사항 - 다른 오브젝트와 충돌 방지)
        Collider itemCollider = worldItem.GetCollider();
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }
        //드는 애니메이션 트리거 설정
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("GetItem");

        }
        StartCoroutine(MoveToHoldPosition(worldItem));

        // 아이템 타입에 따른 처리 효과 다르게하기
        if (worldItem.Item.Type == ItemType.CORE)
        {
            Debug.Log("코어 아이템 획득: " + worldItem.Item.Name);
        }
        else if (worldItem.Item.Type == ItemType.NORMAL)
        {
            Debug.Log("일반 아이템 획득: " + worldItem.Item.Name);
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