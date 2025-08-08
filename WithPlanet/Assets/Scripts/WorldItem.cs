using UnityEngine;

/// <summary>
/// Item을 넣는 공간 프리팹에 컴포넌트로 추가하고 인스펙터에 아이템을 할당한다.
/// </summary>
public class WorldItem : MonoBehaviour
{
    [Header("해당 오브젝트에 할당되는 아이템")]
    [SerializeField] private Item mItem;
    public Item Item
    {
        get
        {
            return mItem; 
        }
    }
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

    private void Update()
    {
        rb.freezeRotation = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rb.isKinematic = true; // 바닥에 닿으면 Rigidbody를 비활성화
            GetCollider().enabled = false; // Collider도 비활성화
        }
    }

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

