using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Minigames.ToxicCleanser
{
    public class MinigameLauncher : MonoBehaviour
    {
        [SerializeField] private string sceneName = "Minigame_ToxicCleanser_Scene";

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
            Debug.Log("Minigame success: give treasure hint");
        }

        private void HandleFail()
        {
            Debug.Log("fail minigame");
        }
    }
}