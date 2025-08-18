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
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private RectTransform timeLine;
        [SerializeField] private HeartsView heartsView;
        [SerializeField] private ResultPanelController resultPanel;
        [SerializeField] private Button exitButton; // 언제든 종료(X)
        [SerializeField] private CanvasGroup fadeOverlay;

        [SerializeField] private Canvas uiCanvas;          // UI Canvas

        private int hearts;
        private float timeLeft;
        public int removedToxic;
        public int spawnedToxic = 0;
        private float timelineFullWidth;

        private readonly List<CommentItem> liveItems = new();

        private void Start()
        {
            exitButton.onClick.AddListener(ExitToMain);

            if (timeLine != null)
            {
                timeLine.anchorMin = new Vector2(0f, 0.5f);
                timeLine.anchorMax = new Vector2(0f, 0.5f);
                timeLine.pivot = new Vector2(0f, 0.5f);
                timelineFullWidth = timeLine.rect.width;           // 시작 폭 저장
                                                                   // 혹시 레이아웃에 의해 0이 될 수 있으니 안전하게 한 번 초기화
                if (timelineFullWidth <= 0f) timelineFullWidth = timeLine.sizeDelta.x;
            }

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
            timerText.text = FormatTime(timeLeft);
            stream.SetSpeed(0f); // 카운트다운 동안 스크롤 정지

            // 스크롤 위치를 맨 위로 고정(초기 미스 방지)
            scrollRect.verticalNormalizedPosition = 1f;

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
            while (timeLeft > 0f && hearts > 0)
            {
                timeLeft -= Time.deltaTime;
                timerText.text = FormatTime(Mathf.Max(0f, timeLeft));

                // 비율(1 → 0)
                float ratio = Mathf.Clamp01(timeLeft / config.totalDurationSec);

                // 목표 폭
                float targetWidth = timelineFullWidth * ratio;

                // 부드럽게 보간(Lerp). 직선감 원하면 MoveTowards로 바꿔도 됨.
                Vector2 sz = timeLine.sizeDelta;
                sz.x = Mathf.Lerp(sz.x, targetWidth, Time.deltaTime * 10f);
                timeLine.sizeDelta = sz;

                yield return null;
            }

            // 6. 종료 판정
            bool success = (timeLeft <= 0f) && (removedToxic >= spawnedToxic) && (hearts > 0);
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
            if (config.comments == null || config.comments.Count == 0)
                return;

            // 1. 댓글 프리팹 높이 측정
            float itemHeight = 140f; // 기본값
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
                int idx = UnityEngine.Random.Range(0, config.comments.Count);
                CommentData c = config.comments[idx];

                var item = pool.Get();
                var rt = (RectTransform)item.transform;
                rt.SetParent(content, false);

                item.Init(c.text, c.isToxic, viewport);
                var swipe = item.GetComponent<SwipeToDelete>();
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
                item.FadeOut(0.05f, () => { pool.Release(item); liveItems.Remove(item); });
                return;
            }

            removedToxic++;
            item.FadeOut(0.05f, () => { pool.Release(item); liveItems.Remove(item); });
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
            if (IsVisibleInViewport(item, padding: 0f))
            {
                //Debug.Log(
               //     $"[MISS-IGNORED] still visible. name='{item.name}', toxic={item.isToxic}, " +
               //     $"worldPos={item.transform.position}, t={Time.time:F2}s");
                return;
            }
            else if (item.isToxic)
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
            liveItems.Remove(item);
            pool.Release(item);
        }

        private bool IsVisibleInViewport(CommentItem item, float padding = 12f)
        {
            if (item == null || viewport == null) return true; // 판단 불가 시 보수적으로 '보임'
            var rt = item.GetComponent<RectTransform>();
            if (rt == null) return true;

            Camera cam = null;

            Rect itemRect = GetScreenRect(rt, cam);
            Rect viewRect = GetScreenRect(viewport, cam);

            // 살짝 여유 패딩(+/-)을 줘서 경계선에서의 오탐 방지
            viewRect.xMin -= padding; viewRect.yMin -= padding;
            viewRect.xMax += padding; viewRect.yMax += padding;

            return itemRect.Overlaps(viewRect, true);
        }

        private static Rect GetScreenRect(RectTransform rt, Camera cam)
        {
            Vector3[] w = new Vector3[4];
            rt.GetWorldCorners(w);
            // 월드 → 스크린
            Vector2 s0 = RectTransformUtility.WorldToScreenPoint(cam, w[0]);
            Vector2 s1 = RectTransformUtility.WorldToScreenPoint(cam, w[1]);
            Vector2 s2 = RectTransformUtility.WorldToScreenPoint(cam, w[2]);
            Vector2 s3 = RectTransformUtility.WorldToScreenPoint(cam, w[3]);

            float xmin = Mathf.Min(Mathf.Min(s0.x, s1.x), Mathf.Min(s2.x, s3.x));
            float xmax = Mathf.Max(Mathf.Max(s0.x, s1.x), Mathf.Max(s2.x, s3.x));
            float ymin = Mathf.Min(Mathf.Min(s0.y, s1.y), Mathf.Min(s2.y, s3.y));
            float ymax = Mathf.Max(Mathf.Max(s0.y, s1.y), Mathf.Max(s2.y, s3.y));

            return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
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

        private string FormatTime(float t)
        {
            int sec = Mathf.CeilToInt(t);
            return $"{sec:D2}s";
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
