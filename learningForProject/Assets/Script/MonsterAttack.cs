using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage = 10f; // 공격력

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            Debug.Log("플레이어와 감지!");

            if (playerHealth != null)
            {
                float before = playerHealth.health;
                playerHealth.health -= damage;
                Debug.Log($"damage: 체력 {before} → {playerHealth.health} (-{damage})");

                if (playerHealth.health <= 0)
                {
                    playerHealth.health = 0;
                    Debug.Log("플레이어 사망");
                }
            }
        }
    }
}
