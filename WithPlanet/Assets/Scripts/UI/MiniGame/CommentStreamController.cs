using UnityEngine;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class CommentStreamController : MonoBehaviour
    {
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private float speedPxPerSec = 250f;

        public bool ReachedEnd
        {
            get
            {
                return scroll.verticalNormalizedPosition <= 0.001f;
            }
        }
        public void SetSpeed(float pxPerSec)
        {
            speedPxPerSec = Mathf.Max(0f, pxPerSec);
        }

        private void Update()
        {
            if (!scroll || speedPxPerSec <= 0f)
            {
                return;
            }

            // Content 높이 대비 px/sec를 normalized 로 환산 
            float h = scroll.content.rect.height;
            if (h <= 0f)
            {
                return;
            }
            float normDelta = (speedPxPerSec / h) * Time.deltaTime;
            scroll.verticalNormalizedPosition = Mathf.Clamp01(scroll.verticalNormalizedPosition - normDelta);
        }
    }
}