using UnityEngine;

/// <summary>
/// Item�� �ִ� ���� �����տ� ������Ʈ�� �߰��ϰ� �ν����Ϳ� �������� �Ҵ��Ѵ�.
/// </summary>
public class WorldItem : MonoBehaviour
{
    [Header("�ش� ������Ʈ�� �Ҵ�Ǵ� ������")]
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
        // ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();

        // Rigidbody�� ������ �߰�
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
            rb.isKinematic = true; // �ٴڿ� ������ Rigidbody�� ��Ȱ��ȭ
            GetCollider().enabled = false; // Collider�� ��Ȱ��ȭ
        }
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    // �ܺο��� Collider�� ������ �� �ְ� ���ִ� �޼���
    public Collider GetCollider()
    {
        return itemCollider;
    }
}

