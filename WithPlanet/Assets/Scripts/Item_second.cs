using UnityEngine;

public class Item_second : MonoBehaviour
{
    public enum Type {Normal,CoreTreasure};

    [Header("������ ����")]
    public string itemName; // ������ �̸�
    public Sprite itemIcon; // ������ ������
    public int value; // ������ ��ġ
    public Type itemType;

    [TextArea(2, 4)]
    public string itemDescription;

    // ���� ������Ʈ ����
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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        { 
            rb.isKinematic = true; // �ٴڿ� ������ Rigidbody�� ��Ȱ��ȭ
            GetCollider().enabled = false; // Collider�� ��Ȱ��ȭ
        }
    }
    // �ܺο��� Rigidbody�� ������ �� �ְ� ���ִ� �޼���
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
