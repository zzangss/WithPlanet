using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Project.Minigames.ToxicCleanser;

public class PlayerAction : MonoBehaviour
{
    //매니저 연결 
    public PlayerMoveController playerMoveController; // 플레이어 이동 컨트롤러
    public InventoryMain inventoryMain; // 카트 인벤토리 
    public DialogueManager dialogueManager;
    public MinigameLauncher minigameLauncher;

    //minigame 실행 관리
    private bool isNearMini = false;

    //NPC 대화 UI 관리
    private GameObject scanObject = null;
    private bool isNearNPC = false;

    //아이템 & 인벤토리 관리 
    public float moveToHoldDuration = 1f;
    public Vector3 holdingOffset = new Vector3(6f, 2f, 0.5f); //플레이어 기본 왼쪽 오프셋
    public Vector3 holdOffset = new Vector3(0f, 18f, 0.5f); // 플레이어 앞쪽 위 오프셋
    public float pickupRange = 8f; // 아이템 획득 범위
    public float dropForce = 5f; // 버릴 때 힘

    public bool hasItem = false; // 아이템을 들고 있는지 여부
    private bool isNearCart = false;

    private ItemDictionary itemDictionary; // 아이템 사전 세이브시스템에서 필요
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

        //아이템 dictionary 가져오기
        itemDictionary = FindObjectOfType<ItemDictionary>();

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
            if (isNearNPC && scanObject != null)
            {
                Debug.Log($"NPC set: {scanObject.name}");
                ObjData scanObjData = scanObject.GetComponent<ObjData>();
                dialogueManager.Action(scanObjData.type);
            }
            else if (dialogueManager.CurrentStage == State.Opening)
            {
                dialogueManager.Action(Type.Boss);
            }

            if (isNearMini)
            {
                minigameLauncher.Launch();
            }

            if (currentItem == null)
            {
                TryPickupItem();
            }
            else
            {
                // 카트가 주변에 없으면 아이템을 버리기
                if (isNearCart)
                {
                    TryPutItem();
                }
                else
                {
                    Debug.Log("아이템 버리기");
                    DropItem();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isNearCart)
            {
                Debug.Log("카트 주변 q 누르기");
                inventoryMain.TryOpenCloseInventory();
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.CompareTag("Cart"))
            {
                isNearCart = true;
                scanObject = other.gameObject;
            }
            if (other.CompareTag("NPC"))
            {
                isNearNPC = true;
                scanObject = other.gameObject;
                Debug.Log($"NPC set: {scanObject.name}");
            }
            if (other.CompareTag("Minigame"))
            {
                isNearMini = true;
                scanObject = other.gameObject;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null) 
        {
            if (other.CompareTag("Cart"))
            {
                isNearCart = false;
                scanObject = null;
            }
            if (other.CompareTag("NPC"))
            {
                isNearNPC = false;
                scanObject = null;
            }
            if (other.CompareTag("Minigame"))
            {
                isNearMini = false;
                scanObject = null;
            }
        }
    }

    //아이템ID 알려주기
    public int getItemID()
    {
        if(currentItem!=null && currentItem.Item != null)
        {
            return currentItem.Item.ItemID;
        }
        
        Debug.LogWarning("현재 아이템이 없습니다.");

        return -1; // 아이템이 없을 경우 -1 반환
    }

    // 세이브시 아이템 설정
    public void setItem(int itemID)
    {
        itemDictionary= FindObjectOfType<ItemDictionary>();

        if(itemID<0)
        {
            ClearcurrentItem();
            Debug.Log("아이템을 들고있지 않았습니다.");
            return;
        }

        //아이템 만들기
        var prefab = itemDictionary.GetItemPrefab(itemID);

        if (prefab == null)
        {
            Debug.LogError($"ID {itemID} 프리팹을 찾을 수 없습니다.");
            return;
        }
        ClearcurrentItem();

        // 1) 인스턴스 생성은 GameObject로 받기
        var go = Instantiate(prefab, itemHoldPoint);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        // 2) 컴포넌트 꺼내서 currentItem에 대입
        currentItem = go.GetComponent<WorldItem>();

        // 물리 비활성화 (Item 스크립트의 Rigidbody 사용)
        Rigidbody itemRb = currentItem.GetRigidbody();
        if (itemRb != null)
        {
            itemRb.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
            itemRb.useGravity = false; // 중력도 끄기
        }

        Debug.Log("아이템 획득");
        currentItem.itemLocation = ItemLocation.PlayerHand; // 아이템 위치를 플레이어 손으로 설정

        //플레이어 애니메이션
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("HasItem", true);
        }

    }

    // 아이템 비우기
    public void ClearcurrentItem()
    {
        if(currentItem != null)
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
            hasItem = false; // 아이템을 들고 있지 않은 상태로 변경
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
            //현재 아이템 칸이 null이라면 카트에 넣을 수 있는 상태
            if (allitems[count].Item == null)
            {
                isPutable = true;
                break;
            }

            //현재 아이템칸이 null이 아니지만, 현재 아이템과 동일하면서 중첩이 가능한 아이템이라면 카트에 넣을 수 있는 상태
            if (allitems[count].Item.ItemID == currentItem.Item.ItemID && allitems[count].Item.CanOverlap)
            {
                isPutable = true;
                break;
            }
        }

        //모든 칸이 null이 아니고, 중첩이 불가능하면 넣을 수 없음
        if (count == allitems.Length)
        {
            isPutable = false;
            return;
        }

       
        // 아이템 얻기
        if (isPutable)
        {
            inventoryMain.AcquireItem(currentItem.Item);

            //아이템 넣는 효과음 재생
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

            hasItem = false; // 아이템을 들고 있지 않은 상태로 변경


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
                hasItem = true; // 아이템을 들고 있는 상태로 변경
                item.itemLocation = ItemLocation.PlayerHand; // 아이템 위치를 플레이어 손으로 설정
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
        hasItem = false; // 아이템을 들고 있지 않은 상태로 변경
        currentItem.itemLocation = ItemLocation.World; // 아이템 위치를 월드로 설정

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