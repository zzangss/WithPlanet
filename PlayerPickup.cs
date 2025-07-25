using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 2f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;

    public Material glowMaterial;       // 효과 줄 메테리얼

    private GameObject heldItem;
    private Rigidbody heldRb;
    private Material defaultMaterial;   // 원래 메테리얼 저장용

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldItem == null)
            {
                TryPickup();
            }
        }

        if (Input.GetKeyDown(dropKey))
        {
            if (heldItem != null)
            {
                Drop();
            }
        }

        if (heldItem != null)
        {
            heldItem.transform.position = holdPoint.position;
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (Collider col in hits)
        {
            if (col.CompareTag("Item"))
            {
                heldItem = col.gameObject;
                heldRb = heldItem.GetComponent<Rigidbody>();

                // 여기부터 메테리얼 변경 부분
                Renderer rend = heldItem.GetComponent<Renderer>();
                if (rend != null)
                {
                    // 원래 메테리얼 저장 (처음 한 번만)
                    if (defaultMaterial == null)
                        defaultMaterial = rend.material;

                    // 효과용 메테리얼로 변경
                    rend.material = glowMaterial;
                }
                // 메테리얼 변경 끝

                if (heldRb != null)
                {
                    heldRb.isKinematic = true;
                    heldItem.transform.SetParent(holdPoint);
                    heldItem.transform.localPosition = Vector3.zero;
                }
                break;
            }
        }
    }

    void Drop()
    {
        if (heldRb != null)
        {
            heldRb.isKinematic = false;
        }

        if (heldItem != null)
        {
            // 메테리얼 원복 부분
            Renderer rend = heldItem.GetComponent<Renderer>();
            if (rend != null && defaultMaterial != null)
            {
                rend.material = defaultMaterial;
            }
            // 메테리얼 원복 끝

            heldItem.transform.SetParent(null);
        }

        heldItem = null;
        heldRb = null;
        defaultMaterial = null;
    }
}
