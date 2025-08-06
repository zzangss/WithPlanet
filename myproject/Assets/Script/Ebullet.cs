using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ebullet : MonoBehaviour
{
    public float lifeTime = 2.0f; // �Ѿ��� ����
    public float damage = 10f; // �Ѿ��� ������
    public float knockbackForce = 20f;   // �˹� ��

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
            Destroy(gameObject); // �浹 �� �Ѿ� ����
        }
        else if(other.CompareTag("Wall"))
        {
            Destroy(gameObject); // �浹 �� �Ѿ� ����
        }
          
    }
}
