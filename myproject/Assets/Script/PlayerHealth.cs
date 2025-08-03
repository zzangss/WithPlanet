using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public bool isInvincible = false; //公利

    public IEnumerator Invincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("公利 惑怕");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }
}