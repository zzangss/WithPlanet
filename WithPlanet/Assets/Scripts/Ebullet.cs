using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ebullet : MonoBehaviour
{
    public float lifeTime = 2.0f; // ÃÑ¾ËÀÇ ¼ö¸í
    public float damage = 10f; // ÃÑ¾ËÀÇ µ¥¹ÌÁö
    public float knockbackForce = 20f;   // ³Ë¹é Èû

    // Start is called before the first frame update
    void Start()
    { 
        Destroy(gameObject, lifeTime); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, transform.position, knockbackForce);
            }
            Destroy(gameObject); // Ãæµ¹ ½Ã ÃÑ¾Ë Á¦°Å
        }
        else if(other.CompareTag("Wall"))
        {
            Destroy(gameObject); // Ãæµ¹ ½Ã ÃÑ¾Ë Á¦°Å
        }
          
    }
}
