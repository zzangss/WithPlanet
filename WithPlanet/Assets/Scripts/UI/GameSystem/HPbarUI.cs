using UnityEngine;
using UnityEngine.UI; // UI 관련 기능을 사용하기 위해 필수입니다.

/// <summary>
/// PlayerHealth 스크립트의 데이터를 받아와 UI에 표시하는 역할을 합니다.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Tooltip("체력 정보를 가져올 플레이어의 PlayerHealth 스크립트입니다.")]
    public PlayerHealth playerHealth;

    [Tooltip("체력을 표시할 녹색 이미지(채워지는 부분)입니다.")]
    public Image healthBarFill;

    void Start()
    {
        // playerHealth가 할당되지 않았다면 씬에서 "Player" 태그로 찾습니다.
        if (playerHealth == null)
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
        }
    }

    // 매 프레임마다 체력 상태를 확인하고 UI에 반영합니다.
    void Update()
    {
        if (playerHealth == null || healthBarFill == null)
        {
            // 필요한 컴포넌트가 없으면 작동하지 않도록 방지합니다.
            return;
        }

        // 현재 체력을 최대 체력으로 나누어 비율(0.0 ~ 1.0)을 계산합니다.
        float healthRatio = playerHealth.health / playerHealth.maxHealth;

       
        healthBarFill.fillAmount = healthRatio;
    }
}
