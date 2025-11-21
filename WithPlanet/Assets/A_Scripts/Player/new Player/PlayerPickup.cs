using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public PlayerMoveController playerMoveController; // 플레이어 이동 컨트롤러
    private List<ItemPickup> detectedItems = new List<ItemPickup>();

    //애니메이션
    private Animator playerAnimator; // 플레이어 애니메이터
    public Transform itemHoldPoint; // 아이템을 들고 있을 위치
    public Transform itemHoldingPoint; // 아이템을 들고 있는 위치
    public float moveToHoldDuration = 1f;
    

    public bool hasItem = false; // 아이템을 들고 있는지 여부

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerMoveController = GetComponent<PlayerMoveController>();

        //아이템을 get할때의 위치 (기본 왼쪽으로 설정)
   


        // 아이템을 들 위치 생성
      
    }

    private void Update()
    {

        // 플레이어가 범위 내에 있을 때만 Q 키 입력을 검사합니다.
        if (detectedItems.Count > 0 && Input.GetKeyDown(KeyCode.Q))
        {
            TryPickupItem();
        }
    }
    void TryPickupItem()
    {
        for (int i = detectedItems.Count - 1; i >= 0; i--)
        {
            ItemPickup itemToPickup = detectedItems[i];
            // ItemPickuped() 호출
            itemToPickup.ItemPickuped();
        }
    }

    

    //감지 아이템 리스트업
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            //Debug.Log("아이템 감지");
            ItemPickup item = other.GetComponent<ItemPickup>();
            if (item != null)
            {
                if (!detectedItems.Contains(item))
                {
                    detectedItems.Add(item);
                }
            }
              
        }
    }
    //감지 아이템 삭제
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemPickup item = other.GetComponent<ItemPickup>();
            if (item != null)
            {
                Debug.Log("아이템 영역 이탈 및 리스트 제거: " + item.gameObject.name);
                //  영역을 벗어나면 리스트에서 제거
                detectedItems.Remove(item);
            }
        }
    }
    /*
    //아이템 획득 애니메이션
    void PickupItem(ItemPickup item)
    {

        Vector3 holdingPosition = holdingOffset;
        if (playerMoveController != null && !playerMoveController.isFlipped)

        {
            holdingPosition.x = -holdingPosition.x; // 6f → -6f로 변경
        }
        itemHoldingPoint.localPosition = holdingPosition;

        //아이템을 플레이어의 손으로 이동
        item.transform.SetParent(itemHoldingPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);

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
    }

    IEnumerator MoveToHoldPosition(ItemPickup item)
    {
        yield return new WaitForSeconds(0.5f);

        item.transform.SetParent(itemHoldPoint);

        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        Vector3 targetPos = Vector3.zero;
        Quaternion targetRot = Quaternion.Euler(45f, 45f, 0f);

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
    
    */
}

