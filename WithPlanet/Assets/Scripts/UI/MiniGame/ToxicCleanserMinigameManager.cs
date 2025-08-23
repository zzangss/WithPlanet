using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class ToxicCleanserMinigameManager : MonoBehaviour
    {
        public static event Action OnMinigameSuccess;
        public static event Action OnMinigameFail;

        [Header("Config & References")]
        [SerializeField] private CommentSet config; // 댓글 세트 (ScriptableObject)
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private CommentStreamController stream;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;
        [SerializeField] private CommentPool pool;

        [Header("UI")]
        [SerializeField] private CountdownController countdownOverlay; // 3-2-1 + Blur
        [SerializeField] private TimelineView timeline;
        [SerializeField] private HeartsView heartsView;
        [SerializeField] private ResultPanelController resultPanel;
        [SerializeField] private Button exitButton; // 언제든 종료(X)
        [SerializeField] private CanvasGroup fadeOverlay;

        [SerializeField] private Canvas uiCanvas;          // UI Canvas

        private int hearts;
        private float timeLeft;
        public int removedToxic;
        public int spawnedToxic = 0;

        private readonly List<CommentItem> liveItems = new();

        private void Start()
        {
            exitButton.onClick.AddListener(ExitToMain);

            StartCoroutine(RunGameLoop());
        }

        private IEnumerator RunGameLoop()
        {
            // 1. 초기화
            hearts = config.hearts;
            timeLeft = config.totalDurationSec;
            spawnedToxic = 0;
            removedToxic = 0;
            heartsView.SetHearts(hearts, config.hearts);
            stream.SetSpeed(0f); // 카운트다운 동안 스크롤 정지

            // 유저 입력으로 스크롤 못 하게 잠금(자동 스크롤만 허용)
            scrollRect.vertical = false;
            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 0f; // 휠 스크롤도 차단

            // 2. 카운트다운
            yield return StartCoroutine(countdownOverlay.StartCountdown(3));

            // 3. 댓글 미리 생성
            SpawnAllForDuration();

            // 4. 스크롤 시작
            stream.SetSpeed(config.scrollSpeed);

            // 5. 타이머 감소
            while (timeLeft >= 0f && hearts > 0)
            {
                // 남은 시간 줄이기 
                timeLeft -= Time.deltaTime;
                
                // 남은시간 타임라인에 반영하기 
                timeline.decreaseTime(timeLeft, config.totalDurationSec);

                yield return null;
            }

            // 6. 종료 판정
            bool success = (timeLeft <= 0f) && (hearts > 0);
            if (success)
            {
                Debug.Log("End");
            }
            EndGame(success);
        }

        /// <summary>
        /// 50초 동안 끊기지 않게 필요한 개수만큼 댓글을 미리 생성
        /// </summary>
        private void SpawnAllForDuration()
        {

            // 1. 댓글 프리팹 높이 측정
            float itemHeight = 100f; // 기본값
            if (pool.PeekPrefab() != null)
            {
                RectTransform rt = pool.PeekPrefab().GetComponent<RectTransform>();
                if (rt != null)
                    itemHeight = rt.sizeDelta.y;
            }

            // 2. Viewport 높이 가져오기
            float viewportHeight = viewport.rect.height;

            // 3. 스크롤해야 할 총 거리 계산
            float totalScrollDistance = config.scrollSpeed * config.totalDurationSec;

            // 4. 필요한 Content 높이
            float requiredContentHeight = viewportHeight + totalScrollDistance;

            // 5. 필요한 댓글 개수
            int neededCount = Mathf.CeilToInt(requiredContentHeight / itemHeight);

            Debug.Log($"[SpawnAllForDuration] Viewport={viewportHeight}, Item={itemHeight}, Need={neededCount}");

            // 6. 댓글 생성
            for (int i = 0; i < neededCount; i++)
            {
                CommentData c;
                int ratio = UnityEngine.Random.Range(0, 10);
                if(ratio < config.toxicSpawnRatio * 10)
                {
                    int idx = UnityEngine.Random.Range(0, config.toxicComments.Count);
                    c = config.toxicComments[idx];
                }
                else
                {
                    int idx = UnityEngine.Random.Range(0, config.untoxicComments.Count);
                    c = config.untoxicComments[idx];
                }

                CommentItem item = pool.Get();
                RectTransform rt = (RectTransform)item.transform;
                rt.SetParent(content, false);

                item.Init(c.text, c.isToxic, viewport);
                SwipeToDelete swipe = item.GetComponent<SwipeToDelete>();
                swipe.OnSwipeDelete = HandleSwipeDelete;
                item.OnMissed = HandleMissed;

                if (c.isToxic) spawnedToxic++;

                liveItems.Add(item);
            }
        }

        /// <summary>
        /// 스와이프 성공 시 호출.
        /// - 선플이면 하트 -1
        /// - 악플이면 removedToxic++
        /// - 시각적으로 즉시 사라지게 처리 후 풀로 반환
        /// </summary>
        private void HandleSwipeDelete(CommentItem item)
        {
            var cg = item.GetComponent<CanvasGroup>();
            if (cg != null) { cg.blocksRaycasts = false; cg.interactable = false; }


            if (!item.isToxic)
            {
                LoseHeart();

                item.setText("removed comment");
                item.ResetPosition();
                return;
            }

            removedToxic++;
            item.isToxic = false;
            item.setText("removed comment");
            item.ResetPosition();
        }

        /// <summary>
        /// 화면 위로 사라져 놓친 경우.
        /// - 악플을 놓치면 하트 -1
        /// - 풀로 반환
        /// </summary>
        private void HandleMissed(CommentItem item)
        {
            if (item == null) return;

            // 화면 안인데 Miss 콜백이 오면 오탐이므로 무시
            if (item.isToxic)
            {
                Debug.Log(
                    $"[MISS] name='{item.name}', toxic=True, worldPos={item.transform.position}, " +
                    $"t={Time.time:F2}s, hearts(before)={hearts}");
                LoseHeart(); // 기존 메서드 사용
            }

            else
            {
                Debug.Log(
                    $"[MISS-NO-PENALTY] name='{item.name}', toxic=False, worldPos={item.transform.position}, t={Time.time:F2}s");
            }

            // 정리: 구독 해제 → 목록 제거 → 풀 반환 (순서 주의)
            try { item.OnMissed -= HandleMissed; } catch { }
        }


        private void LoseHeart()
        {
            hearts = Mathf.Max(0, hearts - 1);
            heartsView.SetHearts(hearts, config.hearts);
            if (hearts == 0)
            {
                // 즉시 종료
                StopAllCoroutines();
                Debug.Log("하트 전부 잃음");
                EndGame(false);
            }
        }


        /// <summary>
        /// 게임 종료 로직 
        /// </summary>
        private void EndGame(bool success)
        {
            // 스트림 정지
            stream.SetSpeed(0f);

            // 남은 아이템 정리
            foreach (var it in liveItems)
                pool.Release(it);
            liveItems.Clear();

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


        // 언제든 종료(X)
        public void ExitToMain()
        {
            StartCoroutine(ExitToMainRoutine());
        }

        private IEnumerator ExitToMainRoutine()
        {
            // 1. 페이드아웃 시작
            yield return StartCoroutine(FadeOut(0.5f));

            // 2. 비동기 씬 언로드 (Additive로 로드된 현재 씬)
            AsyncOperation op = SceneManager.UnloadSceneAsync(gameObject.scene);

            // 3. 언로드 진행 상황 대기
            while (!op.isDone)
            {
                yield return null; // 다음 프레임까지 대기
            }

            Debug.Log("미니게임 씬 언로드 완료!");
        }

        private IEnumerator FadeOut(float duration)
        {
            if (fadeOverlay == null)
                yield break;

            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, time / duration);
                yield return null;
            }
            fadeOverlay.alpha = 1f;
        }
    }
}
