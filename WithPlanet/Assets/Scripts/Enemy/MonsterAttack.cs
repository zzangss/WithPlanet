using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public float damage = 10f;
    public float knockbackForce = 20f;
    public float damageInterval = 0.6f; // 피해를 입힐 간격
    public bool attackEnabled = true;

    private float damageTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (other.CompareTag("Player") && attackEnabled)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f)
            {
                playerHealth.TakeDamage(damage, transform.position, knockbackForce);
                Debug.Log($"플레이어 공격! 딜 : {damage}");
                damageTimer = 0f;
            }
        }
    }
}
