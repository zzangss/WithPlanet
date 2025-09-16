using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class GridItemView : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Image icon;                 // 상단 이미지
        [SerializeField] private Button actionButton;        // 하단 클릭부(싫어요 등)
        [SerializeField] private CanvasGroup canvasGroup;    // 선택(없으면 자동 추가)

        public Image IconImage => icon;
        public Button ActionButton => actionButton;

        public bool IsTarget { get; private set; }           // 이번 웨이브에서 타깃인지

        // 버튼이 눌렸을 때만 알림
        public event Action<GridItemView> OnActionPressed;

        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (actionButton != null)
                actionButton.onClick.AddListener(() => OnActionPressed?.Invoke(this));
        }

        /// <summary>웨이브마다 아이콘/타깃 여부 세팅 + 비주얼 초기화</summary>
        public void Setup(Sprite sprite, bool isTarget)
        {
            IsTarget = isTarget;
            if (icon != null) icon.sprite = sprite;
            ResetVisualState();
        }

        /// <summary>시각적 제거: 레이아웃 고정, 상호작용만 막고 반투명 처리</summary>
        public void ApplyRemovedVisual()
        {
            if (actionButton) actionButton.interactable = false;

            if (icon)
            {
                var c = icon.color;
                icon.color = new Color(c.r, c.g, c.b, 0.35f);
            }
            if (canvasGroup)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }
        }

        /// <summary>다음 웨이브를 위해 초기 상태로 되돌림</summary>
        public void ResetVisualState()
        {
            if (actionButton) actionButton.interactable = true;

            if (icon)
            {
                var c = icon.color;
                icon.color = new Color(c.r, c.g, c.b, 1f);
            }
            if (canvasGroup)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
                canvasGroup.alpha = 1f;
            }
            gameObject.SetActive(true);
        }
    }
}
