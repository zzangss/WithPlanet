using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Minigames.ToxicCleanser
{
    public class MinigameLauncher : Singleton<MinigameLauncher>
    {
        [SerializeField] private string sceneName = "Minigame_ToxicCleanser_Scene";
        public static bool isMiniRunning = false;

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
            {
                isMiniRunning = true;
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }

        private void HandleSuccess()
        {
            Debug.Log("Minigame success: give treasure hint");
        }

        private void HandleFail()
        {
            Debug.Log("fail minigame");
        }
    }
}