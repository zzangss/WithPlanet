using UnityEngine;
using System.Collections.Generic;

public class HealingZone : MonoBehaviour
{
    public float healingAmount = 5f; // �ʴ� ȸ����
    private Dictionary<PlayerHealth, float> healingTimers = new();

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null && !healingTimers.ContainsKey(player))
        {
            healingTimers.Add(player, 0f);
            Debug.Log("�� ����");
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
                Debug.Log($"����Zone: ü�� ȸ�� +{healingAmount}");
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
            Debug.Log("�� ����");
        }
    }
}
