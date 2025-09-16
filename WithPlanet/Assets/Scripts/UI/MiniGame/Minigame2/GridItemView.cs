using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class GridItemView : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Image icon;
        [SerializeField] private Button actionButton;
        [SerializeField] private CanvasGroup canvasGroup;

        public Image IconImage => icon;
        public Button ActionButton => actionButton;

        public bool IsTarget { get; private set; }
        public event Action<GridItemView> OnActionPressed;

        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            if (actionButton != null)
                actionButton.onClick.AddListener(() => OnActionPressed?.Invoke(this));
        }

        /// <summary>
        /// 게시물을 셋업한다. 
        /// </summary>
        /// <param name="sprite">게시물 이미지</param>
        /// <param name="isTarget">true : 타깃 게시물</param>
        public void Setup(Sprite sprite, bool isTarget)
        {
            IsTarget = isTarget;
            if (icon != null) icon.sprite = sprite;
            ResetVisualState();
        }

        /// <summary>
        ///  지워진 게시물을 이미지로 표현하고 상호작용을 막는다.
        /// </summary>
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

        /// <summary>
        /// 게시물을 활성화한다. (이미지 복구 + 상호작용 가능)
        /// </summary>
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

        /// <summary>
        /// 일반 게시물을 타겟 게시물로 바꿀 수 있다.
        /// </summary>
        /// <param name="targetSprite"></param>
        public void ForceSetTarget(Sprite targetSprite)
        {
            IsTarget = true;
            if (icon) icon.sprite = targetSprite;
            if (actionButton) actionButton.interactable = true;
        }
    }
}
