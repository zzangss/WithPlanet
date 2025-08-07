using UnityEngine;

public class Item_second : MonoBehaviour
{
    public enum Type {Normal,CoreTreasure};

    [Header("아이템 정보")]
    public string itemName; // 아이템 이름
    public Sprite itemIcon; // 아이템 아이콘
    public int value; // 아이템 가치
    public Type itemType;

    [TextArea(2, 4)]
    public string itemDescription;

    // 물리 컴포넌트 참조
    private Rigidbody rb;
    private Collider itemCollider;
    void Awake()
    {
        // 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();

        // Rigidbody가 없으면 추가
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        { 
            rb.isKinematic = true; // 바닥에 닿으면 Rigidbody를 비활성화
            GetCollider().enabled = false; // Collider도 비활성화
        }
    }
    // 외부에서 Rigidbody에 접근할 수 있게 해주는 메서드
    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    // 외부에서 Collider에 접근할 수 있게 해주는 메서드
    public Collider GetCollider()
    {
        return itemCollider;
    }
}
