using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Project.Minigames.ToxicCleanser
{
    public class Minigame2Manager : MonoBehaviour
    {
        public static event Action OnMinigameSuccess;
        public static event Action OnMinigameFail;

        [Header("Config")]
        [SerializeField] private ImageSet config;

        [Header("UI")]
        [SerializeField] private CountdownController countdownOverlay; // StartCountdown(3)
        [SerializeField] private TimelineView timeline;          // decreaseTime(timeLeft, total)
        [SerializeField] private ResultPanelController resultPanel;    // ShowSuccess/ShowFail
        [SerializeField] private Button exitButton;
        [SerializeField] private Button resultExitButton;
        [SerializeField] private CanvasGroup fadeOverlay;
        [SerializeField] private UIShake backgroundShake;   // (선택)

        [Header("Board")]
        [SerializeField] private GridBoard board;
        [SerializeField, Range(2, 6)] private int gridSize = 3;

        [Header("Refresh FX (블러 오버레이 Image)")]
        [SerializeField] private Image refreshOverlay;
        [SerializeField, Range(0.05f, 0.6f)] private float refreshFxDuration = 0.25f;

        [Header("Progress UI")]
        [SerializeField] private TMP_Text progressText; // "현재/목표"

        // === 내부 상태 ===
        private float timeLeft;
        private float waveTimer;
        private float flipTimer;

        private int clearedTotal;
        private int targetCountThisWave;

        private bool isRefreshing;
        private readonly List<GridItemView> cells = new();
        private System.Random rnd = new();

        private float RefreshInterval => config.refreshInterval;
        private float TargetRatio => config.targetSpawnRatio;
        private float FlipProbPerSecond => config.flipProbPerSecond;
        private float FlipCheckInterval => config.flipCheckInterval;
        private int MaxFlipsPerTick => Mathf.Max(0, config.maxFlipsPerTick);
        private int MaxTargetsOnBoard => config.maxTargetsOnBoard;

        private void Start()
        {
            exitButton.onClick.AddListener(ExitToMain);
            resultExitButton.onClick.AddListener(ExitToMain);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            board.Build(gridSize);
            cells.Clear();
            foreach (var c in board.Cells)
            {
                c.OnActionPressed += HandleActionPressed;
                cells.Add(c);
            }

            if (refreshOverlay != null)
            {
                var cc = refreshOverlay.color;
                refreshOverlay.color = new Color(cc.r, cc.g, cc.b, 0f);
                refreshOverlay.raycastTarget = false;
            }

            StartCoroutine(RunGameLoop());
        }

        private IEnumerator RunGameLoop()
        {
            timeLeft = config.totalDurationSec;
            clearedTotal = 0;
            UpdateProgressText();
            timeline.decreaseTime(timeLeft, config.totalDurationSec);

            yield return StartCoroutine(countdownOverlay.StartCountdown(3));

            waveTimer = 0f;
            flipTimer = 0f;
            SetUpNewWave();

            while (timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                timeline.decreaseTime(timeLeft, config.totalDurationSec);

                waveTimer += Time.deltaTime;
                if (waveTimer >= RefreshInterval && !isRefreshing)
                    StartCoroutine(RefreshWaveRoutine());

                flipTimer += Time.deltaTime;
                if (flipTimer >= FlipCheckInterval && FlipProbPerSecond > 0f && MaxFlipsPerTick > 0)
                {
                    flipTimer = 0f;
                    TryFlipNeutralsToTargets();
                }

                if (clearedTotal >= config.targetGoal)
                {
                    EndGame(true);
                    yield break;
                }

                yield return null;
            }

            EndGame(clearedTotal >= config.targetGoal);
        }

        /// <summary>
        /// 새로고침 루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator RefreshWaveRoutine()
        {
            isRefreshing = true;
            waveTimer = 0f;

            SetUpNewWave();

            if (refreshOverlay && refreshFxDuration > 0f)
            {
                yield return StartCoroutine(FadeImage(refreshOverlay, 1f, 0f, refreshFxDuration));
            }

            isRefreshing = false;
        }
        /// <summary>
        /// 새로운 게시물을 셋업해주는 함수 (새로고침)
        /// </summary>
        private void SetUpNewWave()
        {
            int total = gridSize * gridSize; // 총 게시물 개수 

            targetCountThisWave = Mathf.Clamp(Mathf.RoundToInt(TargetRatio * total), 0, total);

            var idx = new List<int>(total);
            for (int i = 0; i < total; i++)
            {
                idx.Add(i);
            }
            Shuffle(idx);

            // 해시셋을 이용해 인덱스로 타깃인지 아닌지 확인 가능 
            var targetSet = new HashSet<int>();
            for (int i = 0; i < targetCountThisWave; i++) 
            { 
                targetSet.Add(idx[i]); 
            }

            // 하나씩 게시물 셋업
            for (int i = 0; i < total; i++)
            {
                var cell = cells[i];
                bool isTarget = targetSet.Contains(i); // i번 게시물이 타깃인지 판단 
                Sprite sprite = PickRandom(isTarget ? config.targetSprites : config.neutralSprites); // isTarget값에 따라 각 스프라이트 세트에서 랜덤으로 이미지 가져오기
                cell.Setup(sprite, isTarget);
                cell.ResetVisualState();
            }
        }

        /// <summary>
        /// 기본 이미지를 타깃 이미지로 전환하는 함수 
        /// </summary>
        private void TryFlipNeutralsToTargets()
        {
            // 현재 활성 타깃 수(아직 제거되지 않아 Action 가능 상태)
            int activeTargets = CountActiveTargets();
            int allowedExtra = (MaxTargetsOnBoard > 0) ? Mathf.Max(0, MaxTargetsOnBoard - activeTargets) : int.MaxValue;
            if (allowedExtra <= 0) return;

            var candidates = new List<GridItemView>();
            foreach (var c in cells)
            {
                if (!c.IsTarget && c.ActionButton != null && c.ActionButton.interactable)
                    candidates.Add(c);
            }
            if (candidates.Count == 0) return;

            float p = Mathf.Clamp01(FlipProbPerSecond * FlipCheckInterval);
            int flips = 0;
            Shuffle(candidates);
            foreach (var cell in candidates)
            {
                if (flips >= MaxFlipsPerTick) break;
                if (flips >= allowedExtra) break;

                if (UnityEngine.Random.value <= p)
                {
                    var newSprite = PickRandom(config.targetSprites);
                    cell.ForceSetTarget(newSprite);
                    flips++;
                }
            }
        }

        /// <summary>
        /// 현재 활성 상태인 타깃 게시물 개수 카운트
        /// </summary>
        /// <returns></returns>
        private int CountActiveTargets()
        {
            int n = 0;
            foreach (var c in cells)
            {
                if (c.IsTarget && c.ActionButton != null && c.ActionButton.interactable)
                {
                    n++;
                }
            }
            return n;
        }

        /// <summary>
        /// 게시물 삭제를 눌렀을 때 동작 함수
        /// </summary>
        /// <param name="cell"></param>
        private void HandleActionPressed(GridItemView cell)
        {
            if (cell == null)
            {
                return;
            }
            if (cell.IsTarget)
            {
                cell.ApplyRemovedVisual();
                clearedTotal++;
                UpdateProgressText();
            }
            else
            {
                cell.ApplyRemovedVisual();

                // 오클릭: 남은 시간 감소
                timeLeft = Mathf.Max(0f, timeLeft - config.penaltySecondsOnWrongClick);
                backgroundShake?.Play(); 
            }
        }

        /// <summary>
        /// 현재까지 지운 타깃 개수를 나타낸다.
        /// </summary>
        private void UpdateProgressText()
        {
            if (progressText != null)
                progressText.text = $"{clearedTotal} / {config.targetGoal}";
        }

        /// <summary>
        /// 성공/실패 시 결과 패널을 보여주고 미니게임 성공/실패 이벤트를 수행한다. 
        /// </summary>
        /// <param name="success">true:게임성공</param>
        private void EndGame(bool success)
        {
            if (success) 
            { 
                resultPanel.ShowSuccess(); 
                OnMinigameSuccess?.Invoke(); 
            }
            else 
            { 
                resultPanel.ShowFail(); 
                OnMinigameFail?.Invoke(); 
            }
        }

        // util
        private IEnumerator FadeImage(Image img, float fromA, float toA, float duration)
        {
            Color baseC = img.color;
            img.raycastTarget = true;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(fromA, toA, t / duration);
                img.color = new Color(baseC.r, baseC.g, baseC.b, a);
                yield return null;
            }
            img.color = new Color(baseC.r, baseC.g, baseC.b, toA);
            img.raycastTarget = toA > 0.01f;
        }

        /// <summary>
        /// 리스트를 무작위로 섞는다. Fisher-Yates 알고리즘 적용
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">대상 리스트</param>
        private void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// 주어진 리스트의 스프라이트를 랜덤 리턴
        /// </summary>
        /// <param name="list">스프라이트 가져올 리스트</param>
        /// <returns></returns>
        private Sprite PickRandom(IList<Sprite> list)
        {
            if (list == null || list.Count == 0) return null;
            int i = UnityEngine.Random.Range(0, list.Count);
            return list[i];
        }

        public void ExitToMain()
        {
            MinigameLauncher.isMiniRunning = false;
            StartCoroutine(ExitToMainRoutine());
        }
        private IEnumerator ExitToMainRoutine()
        {
            yield return StartCoroutine(FadeOut(0.5f));
            AsyncOperation op = SceneManager.UnloadSceneAsync(gameObject.scene);
            while (!op.isDone) yield return null;
        }
        private IEnumerator FadeOut(float duration)
        {
            if (fadeOverlay == null) yield break;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, t / duration);
                yield return null;
            }
            fadeOverlay.alpha = 1f;
        }
    }
}
