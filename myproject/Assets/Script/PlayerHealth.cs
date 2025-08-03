using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public bool isInvincible = false; //����

    public IEnumerator Invincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("���� ����");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }
}