using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage = 10f; // 공격력
    public float knockbackForce = 20f; // 넉백 힘

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();

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

            if (playerRb != null)
            { 
                knockback(playerRb,transform.position, knockbackForce); 
            }

            
        }
    }
    private void knockback(Rigidbody playerRb, Vector3 attackPosition, float force)
    {
        
        Vector3 horizontal = (playerRb.transform.position - attackPosition);
        horizontal.y = 0f; // 수직 방향은 제외
        horizontal.Normalize();

        float upwardForce = 3f; // 위로 튕기는 힘
        playerRb.velocity = Vector3.zero;

        playerRb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

        playerRb.AddForce(horizontal * force, ForceMode.Impulse);

        Debug.Log("튕기기!");
    }
}
