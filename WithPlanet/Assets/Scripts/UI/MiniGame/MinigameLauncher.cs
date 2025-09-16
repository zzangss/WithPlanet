using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Minigames.ToxicCleanser
{
    public class MinigameLauncher : Singleton<MinigameLauncher>
    {
        public static bool isMiniRunning = false;

        [SerializeField] private string[] sceneNames =
        {
            "MinigameScene",
            "MinigameScene2"
        };

        [SerializeField] private int sceneIndex = 0;
        private bool transitioning = false; // 씬 전환중 여부를 저장. 중복 실행 방지용

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
            if (isMiniRunning || transitioning) return;
            if (sceneNames == null || sceneNames.Length == 0) return;
            if (sceneIndex < 0 || sceneIndex >= sceneNames.Length) return;

            isMiniRunning = true;
            SceneManager.LoadScene(sceneNames[sceneIndex], LoadSceneMode.Additive);
        }

        private void HandleSuccess()
        {
            if (transitioning) return;
            Debug.Log("Minigame success");

            // 마지막이면 정리하고 종료
            if (sceneIndex + 1 >= sceneNames.Length)
            {
                return;
            }

            sceneIndex++;
        }

        private void HandleFail()
        {
            Debug.Log("fail minigame");
        }
    }
}
