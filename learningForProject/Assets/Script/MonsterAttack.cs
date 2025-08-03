using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage = 10f; // ���ݷ�

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
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
        }
    }
}
