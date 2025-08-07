using UnityEngine;
using System.Collections.Generic;

public class HealingZone : MonoBehaviour
{
    public float healingAmount = 5f; // 초당 회복량
    private Dictionary<PlayerHealth, float> healingTimers = new();

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null && !healingTimers.ContainsKey(player))
        {
            healingTimers.Add(player, 0f);
            Debug.Log("힐 시작");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null && healingTimers.ContainsKey(player))
        {
            healingTimers[player] += Time.deltaTime;

            if (healingTimers[player] >= 1f)
            {
                player.Heal(healingAmount); 
                Debug.Log($"힐링Zone: 체력 회복 +{healingAmount}");
                healingTimers[player] = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null && healingTimers.ContainsKey(player))
        {
            healingTimers.Remove(player);
            Debug.Log("힐 종료");
        }
    }
}
