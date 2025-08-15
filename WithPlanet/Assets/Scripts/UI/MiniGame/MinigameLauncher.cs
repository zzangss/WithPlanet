using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Minigames.ToxicCleanser
{
    public class MinigameLauncher : MonoBehaviour
    {
        [SerializeField] private string sceneName = "Minigame_ToxicCleanser_Scene";
        [SerializeField] private NpcMadnessController madnessController; // 메인 씬의 NPC에 부착

        private void OnEnable()
        {
            ToxicCleanserMinigameManager.OnMinigameSuccess += HandleSuccess;
            ToxicCleanserMinigameManager.OnMinigameFail += HandleFail;
        }

        private void OnDisable()
        {
            ToxicCleanserMinigameManager.OnMinigameSuccess -= HandleSuccess;
            ToxicCleanserMinigameManager.OnMinigameFail -= HandleFail;
        }

        public void Launch()
        {
            if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        private void HandleSuccess()
        {
            // TODO: 보물 힌트 지급(대사 업데이트/아이템 지급 등)
            Debug.Log("Minigame success: give treasure hint");
        }

        private void HandleFail()
        {
            if (madnessController != null)
                madnessController.TriggerMadness(10f, 1.5f, 20);
        }
    }
}