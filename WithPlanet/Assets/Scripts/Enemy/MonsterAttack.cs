using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public float damage = 10f;
    public float knockbackForce = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, transform.position, knockbackForce);
                Debug.Log("�÷��̾� ����!");
            }
        }
    }
}
