using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private ImageSet config;                 // 미니게임2 전용 설정

        [Header("UI (기존 구현 재사용)")]
        [SerializeField] private CountdownController countdownOverlay; // StartCountdown(3)
        [SerializeField] private TimelineView timeline;               // decreaseTime(timeLeft, total)
        [SerializeField] private HeartsView heartsView;               // SetHearts(curr, max)
        [SerializeField] private ResultPanelController resultPanel;   // ShowSuccess/ShowFail
        [SerializeField] private Button exitButton;
        [SerializeField] private Button resultExitButton;
        [SerializeField] private CanvasGroup fadeOverlay;
        [SerializeField] private UIShake backgroundShake;

        [Header("Board")]
        [SerializeField] private GridBoard board;                // GridLayoutGroup + GridItemView 프리팹
        [SerializeField, Range(2, 6)] private int gridSize = 3;

        [Header("Override (씬에서 즉석 튜닝용)")]
        [SerializeField] private bool overrideRefreshInterval = false;
        [SerializeField, Min(0.3f)] private float refreshIntervalOverride = 1.5f;

        [SerializeField] private bool overrideTargetRatio = false;
        [SerializeField, Range(0f, 1f)] private float targetRatioOverride = 0.4f;

        // 내부 상태
        private int hearts;
        private float timeLeft;
        private float waveTimer;

        private int targetCountThisWave;
        private int clearedTargetThisWave;

        private readonly List<GridItemView> cells = new();
        private System.Random rnd = new();

        private float RefreshInterval => overrideRefreshInterval ? refreshIntervalOverride : config.refreshInterval;
        private float TargetRatio => overrideTargetRatio ? targetRatioOverride : config.targetSpawnRatio;

        private void Start()
        {
            exitButton.onClick.AddListener(ExitToMain);
            resultExitButton.onClick.AddListener(ExitToMain);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 보드 생성 및 셀 구독
            board.Build(gridSize);
            cells.Clear();
            foreach (var c in board.Cells)
            {
                c.OnActionPressed += HandleActionPressed;  // ⬅️ 버튼만 판정
                cells.Add(c);
            }

            StartCoroutine(RunGameLoop());
        }

        private IEnumerator RunGameLoop()
        {
            // 1) 초기화 — 기존 HUD API 시그니처 그대로
            hearts = config.hearts;
            timeLeft = config.totalDurationSec;
            heartsView.SetHearts(hearts, config.hearts);
            timeline.decreaseTime(timeLeft, config.totalDurationSec);

            // 2) 카운트다운
            yield return StartCoroutine(countdownOverlay.StartCountdown(3));

            // 3) 첫 웨이브 시작
            waveTimer = 0f;
            SpawnNewWave();

            // 4) 메인 루프
            while (timeLeft >= 0f && hearts > 0)
            {
                timeLeft -= Time.deltaTime;
                timeline.decreaseTime(timeLeft, config.totalDurationSec);

                waveTimer += Time.deltaTime;
                if (waveTimer >= RefreshInterval)
                {
                    // 타깃을 다 못 지웠으면 패널티
                    if (clearedTargetThisWave < targetCountThisWave)
                    {
                        LoseHeart();
                        if (hearts <= 0) break;
                        backgroundShake?.Play();
                    }
                    SpawnNewWave();
                }

                yield return null;
            }

            // 5) 종료
            bool success = (timeLeft <= 0f) && (hearts > 0);
            EndGame(success);
        }

        private void SpawnNewWave()
        {
            waveTimer = 0f;
            int total = gridSize * gridSize;

            targetCountThisWave = Mathf.Clamp(Mathf.RoundToInt(TargetRatio * total), 0, total);
            clearedTargetThisWave = 0;

            // 인덱스 섞기
            var idx = new List<int>(total);
            for (int i = 0; i < total; i++) idx.Add(i);
            for (int i = total - 1; i > 0; i--)
            {
                int j = rnd.Next(0, i + 1);
                (idx[i], idx[j]) = (idx[j], idx[i]);
            }

            var targetSet = new HashSet<int>();
            for (int i = 0; i < targetCountThisWave; i++) targetSet.Add(idx[i]);

            // 셀 세팅
            for (int i = 0; i < total; i++)
            {
                var cell = cells[i];
                bool isTarget = targetSet.Contains(i);

                Sprite sprite = PickRandom(isTarget ? config.targetSprites : config.neutralSprites);
                cell.Setup(sprite, isTarget);          // 이미지 + 타깃 여부
                cell.ResetVisualState();               // 버튼/투명도 초기화
            }
        }

        private void HandleActionPressed(GridItemView cell)
        {
            if (cell == null) return;

            if (cell.IsTarget)
            {
                // 올바른 제거: 버튼만 눌렸을 때 시각적 제거
                cell.ApplyRemovedVisual();
                clearedTargetThisWave++;

                // 퍼펙트 → 즉시 다음 웨이브
                if (clearedTargetThisWave >= targetCountThisWave)
                {
                    SpawnNewWave();
                }
            }
            else
            {
                // 중립에 눌렀다면 오클릭 패널티
                LoseHeart();
                backgroundShake?.Play();
                if (hearts <= 0)
                {
                    StopAllCoroutines();
                    EndGame(false);
                }
            }
        }

        private void LoseHeart()
        {
            hearts = Mathf.Max(0, hearts - 1);
            heartsView.SetHearts(hearts, config.hearts); // ✅ 기존 시그니처 유지
        }

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

        // — 튜닝용(슬라이더에서 연결 가능) —
        public void SetTargetRatio(float ratio) => overrideTargetRatio = Mathf.Clamp01(ratio);
        public void SetRefreshInterval(float sec) => overrideRefreshInterval = (sec >= 0.3f);

        // — 씬 복귀(기존 로직 유지) —
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
            Debug.Log("미니게임2 씬 언로드 완료!");
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

        private Sprite PickRandom(IList<Sprite> list)
        {
            if (list == null || list.Count == 0) return null;
            int i = rnd.Next(0, list.Count);
            return list[i];
        }
    }
}
