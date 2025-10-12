using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project.Minigames.ToxicCleanser
{

    public class TimelineView : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private RectTransform timeLine;

        private float timelineFullWidth;

        // Start is called before the first frame update
        void Start()
        {
            if (timeLine != null)
            {
                timeLine.anchorMin = new Vector2(0f, 0.5f);
                timeLine.anchorMax = new Vector2(0f, 0.5f);
                timeLine.pivot = new Vector2(0f, 0.5f);
                timelineFullWidth = timeLine.rect.width;           // 시작 폭 저장
                                                                   // 혹시 레이아웃에 의해 0이 될 수 있으니 안전하게 한 번 초기화
                if (timelineFullWidth <= 0f) timelineFullWidth = timeLine.sizeDelta.x;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 남은 시간에 따라 타임라인을 감소
        /// </summary>
        public void decreaseTime(float timeLeft, int totalDuration)
        {
            // 최소 0s 남도록 셋팅 
            timerText.text = FormatTime(Mathf.Max(0f, timeLeft));

            // 남은시간 / 총시간을 전체 길이에 곱하여 타임라인 제어
            float ratio = Mathf.Clamp01(timeLeft / totalDuration);
            float targetWidth = timelineFullWidth * ratio;

            // 선형보간 함수로 타임라인이 부드럽게 줄어들도록 
            Vector2 sz = timeLine.sizeDelta;
            sz.x = Mathf.Lerp(sz.x, targetWidth, Time.deltaTime * 10f);
            timeLine.sizeDelta = sz;

        }

        /// <summary>
        /// float 시간을 정수로 보정 
        /// </summary>
        private string FormatTime(float t)
        {
            int sec = Mathf.CeilToInt(t);
            return $"{sec:D2}s";
        }
    }
}