using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public bool isInvincible = false;

    public float knockbackUpForce = 3f;

    public void TakeDamage(float damage, Vector3 attackerPosition, float knockbackForce)
    {
        if (isInvincible) return;

        float before = health;
        health -= damage;
        Debug.Log($"피해: 체력 {before} → {health} (-{damage})");

        if (health <= 0)
        {
            health = 0;
            Debug.Log("플레이어 사망");
        }

        StartCoroutine(Invincibility(1.0f));

        ApplyKnockback(attackerPosition, knockbackForce);
    }

    private void ApplyKnockback(Vector3 attackerPosition, float force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 direction = (transform.position - attackerPosition).normalized;
        direction.y = 0f;

        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * knockbackUpForce, ForceMode.Impulse);
        rb.AddForce(direction * force, ForceMode.Impulse);

       
    }

    public IEnumerator Invincibility(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        
    }
}
