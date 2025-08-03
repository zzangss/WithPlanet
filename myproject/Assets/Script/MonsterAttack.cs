using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage = 10f; // ���ݷ�
    public float knockbackForce = 20f; // �˹� ��

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();

            Debug.Log("�÷��̾�� ����!");

            if (playerHealth != null)
            {
                float before = playerHealth.health;
                playerHealth.health -= damage;
                Debug.Log($"damage: ü�� {before} �� {playerHealth.health} (-{damage})");

                if (playerHealth.health <= 0)
                {
                    playerHealth.health = 0;
                    Debug.Log("�÷��̾� ���");
                }
            }

            if (playerRb != null)
            { 
                knockback(playerRb,transform.position, knockbackForce); 
            }

            
        }
    }
    private void knockback(Rigidbody playerRb, Vector3 attackPosition, float force)
    {
        
        Vector3 horizontal = (playerRb.transform.position - attackPosition);
        horizontal.y = 0f; // ���� ������ ����
        horizontal.Normalize();

        float upwardForce = 3f; // ���� ƨ��� ��
        playerRb.velocity = Vector3.zero;

        playerRb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

        playerRb.AddForce(horizontal * force, ForceMode.Impulse);

        Debug.Log("ƨ���!");
    }
}
