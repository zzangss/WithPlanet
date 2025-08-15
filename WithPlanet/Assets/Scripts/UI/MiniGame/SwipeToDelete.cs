using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Minigames.ToxicCleanser
{
    public class SwipeToDelete : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public Action<CommentItem> OnSwipeDelete;

        [SerializeField] private RectTransform rect;
        [SerializeField] private float thresholdX = 150f;  // 이 이상 우측으로 이동 시 삭제
        [SerializeField] private float maxAngle = 0f;      // 살짝 회전 효과
        [SerializeField] private float springBackSpeed = 10f;

        private Vector2 startPos;
        private bool dragging;
        private CommentItem item;

        private void Reset()
        {
            rect = GetComponent<RectTransform>();
            item = GetComponent<CommentItem>();
        }

        private void Awake()
        {
            if (!rect) rect = GetComponent<RectTransform>();
            if (!item) item = GetComponent<CommentItem>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("드래그 시작");
            dragging = true;
            startPos = rect.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            Vector2 delta = eventData.delta;
            Vector2 pos = rect.anchoredPosition + new Vector2(delta.x, 0f);
            // 좌우만 이동
            rect.anchoredPosition = pos;
            float angle = Mathf.Clamp(rect.anchoredPosition.x * 0.05f, -maxAngle, maxAngle);
            rect.localRotation = Quaternion.Euler(0, 0, -angle);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!dragging) return;
            dragging = false;
            bool pass = rect.anchoredPosition.x - startPos.x >= thresholdX;
            if (pass)
            {
                OnSwipeDelete?.Invoke(item);
            }
            else
            {
                // 원위치 스프링백
                StopAllCoroutines();
                StartCoroutine(SpringBack());
            }
        }

        private System.Collections.IEnumerator SpringBack()
        {
            Vector2 p0 = rect.anchoredPosition;
            Quaternion r0 = rect.localRotation;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * springBackSpeed;
                rect.anchoredPosition = Vector2.Lerp(p0, startPos, t);
                rect.localRotation = Quaternion.Slerp(r0, Quaternion.identity, t);
                yield return null;
            }
        }
    }
}