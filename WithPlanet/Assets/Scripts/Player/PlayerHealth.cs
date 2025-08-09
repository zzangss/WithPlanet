using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float health;

    [Header("Invincibility")]
    public bool isInvincible = false;
    public float invincibilityDuration = 1.0f;

    [Header("Knockback")]
    public float upwardForce = 2f;

    [Header("Visual Feedback")]
    public Renderer playerRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    private Color originalColor;
    private Rigidbody rb;

    void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody>();

        if (playerRenderer == null)
            playerRenderer = GetComponentInChildren<Renderer>();

        if (playerRenderer != null)
            originalColor = playerRenderer.material.color;
    }

    public void TakeDamage(float damage, Vector3 attackerPosition, float knockbackForce)
    {
        if (isInvincible) return;

        health -= damage;
        Debug.Log($"피해: -{damage}, 남은 체력: {health}");

        if (health <= 0)
        {
            Die();
            return;
        }

        ApplyKnockback(attackerPosition, knockbackForce);
        StartCoroutine(InvincibilityCoroutine());
    }

    private void ApplyKnockback(Vector3 attackerPosition, float knockbackForce)
    {
        if (rb == null) return;

        Vector3 knockDirection = (transform.position - attackerPosition).normalized;
        knockDirection.y = 0.2f;
        knockDirection.Normalize();

        Vector3 knockback = knockDirection * knockbackForce;
        knockback.y = upwardForce;

        rb.AddForce(knockback, ForceMode.Impulse);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        StartCoroutine(FlashEffect());
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private IEnumerator FlashEffect()
    {
        if (playerRenderer == null) yield break;

        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            playerRenderer.material.color = damageColor;
            yield return new WaitForSeconds(flashDuration);

            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(flashDuration);

            elapsed += flashDuration * 2;
        }

        playerRenderer.material.color = originalColor;
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        Debug.Log($"회복: +{amount}, 현재 체력: {health}");
    }
}
