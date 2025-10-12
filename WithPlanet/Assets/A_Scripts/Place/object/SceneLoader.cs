using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("이 오브젝트가 연결할 씬 이름")]
    [SerializeField] private string targetSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어만 작동
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("targetSceneName이 설정되지 않았습니다.");
            }
        }
    }
}
