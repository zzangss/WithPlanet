using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDamage : MonoBehaviour
{
    [Tooltip("플레이어에게 입힐 데미지 양")]
    public int damageAmount = 10;

    [Tooltip("데미지를 반복해서 입히는 시간 간격 (쿨다운)")]
    public float damageCooldown = 1f;

    private bool isDamage = true; // 데미지를 줄 수 있는지 여부를 확인

    [SerializeField] private PlayerHealth playerHealth;



    // Trigger 안에 다른 Collider가 들어왔을 때 호출
    void OnTriggerEnter(Collider other)
    {
      

        if (other.CompareTag("Player") )
        {

           

            if (playerHealth == null)
            {
                playerHealth = other.GetComponent<PlayerHealth>();
            }

            //damage주기 
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, transform.position,20);
                
                // 데미지를 준 후에는 잠시 데미지를 못 주도록 쿨다운 코루틴을 시작
                StartCoroutine(DamageCooldown());
            }
        }
    }

    // 쿨다운을 처리하는 코루틴
    private IEnumerator DamageCooldown()
    {
        isDamage = false; // 데미지를 줄 수 없는 상태로 변경
        yield return new WaitForSeconds(damageCooldown); // 설정된 시간만큼 대기
        isDamage = true; // 다시 데미지를 줄 수 있는 상태로 복귀
    }

}
