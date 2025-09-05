using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class CommentItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text body; 
        [SerializeField] private Image bg;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rect;
        [SerializeField] private Image portrait;
        [SerializeField] private TMP_Text nicName;

        [NonSerialized] public bool isToxic;
        public Action<CommentItem> OnMissed;

        private RectTransform viewport;
        private bool initialized;

        public void Init(string text, bool toxic, RectTransform vp, Sprite portrait, string nicName)
        {
            body.text = text;
            isToxic = toxic;
            viewport = vp;
            if (canvasGroup != null) 
            { 
                canvasGroup.alpha = 1f; 
            }
            this.portrait.sprite = portrait;
            this.nicName.text = nicName;
            initialized = true;
        }

        private void Reset()
        {
            rect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            body = GetComponentInChildren<TMP_Text>();
        }
        private void Update()
        {
            if (!initialized || rect == null || viewport == null) return;
            // 뷰포트 상단을 크게 벗어나면 미스로 간주 (여유 오프셋 50px)
            Vector3[] vpWorld = new Vector3[4];
            viewport.GetWorldCorners(vpWorld);
            Vector3[] meWorld = new Vector3[4];
            rect.GetWorldCorners(meWorld);

            // 완전히 위로 사라진 경우(하단 y가 뷰포트 상단 y보다 높음)
            if (meWorld[0].y > vpWorld[2].y)
            {
                OnMissed?.Invoke(this);
                initialized = false; // 중복 호출 방지
            }
        }

        public void FadeOut(float duration, Action onComplete)
        {
            if (canvasGroup == null) { onComplete?.Invoke(); return; }
            StartCoroutine(FadeRoutine(duration, onComplete));
        }

        private System.Collections.IEnumerator FadeRoutine(float t, Action onComplete)
        {
            float a0 = canvasGroup.alpha;
            float time = 0f;
            while (time < t)
            {
                time += Time.deltaTime;
                float k = Mathf.Clamp01(time / t);
                canvasGroup.alpha = Mathf.Lerp(a0, 0f, k);
                yield return null;
            }
            onComplete?.Invoke();
        }

        public void MarkDeletedVisual()
        {
            bg.color = Color.gray;
        }

        public void setText(string text)
        {
            body.text = text;
        }

        // CommentItem.cs (또는 별도 스크립트에)
        public void ResetPosition()
        {
            var swipe = GetComponent<SwipeToDelete>();
            if (swipe != null)
            {
                swipe.ForceSpringBack();
            }
        }
    }
}