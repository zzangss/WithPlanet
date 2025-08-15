using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Project.Minigames.ToxicCleanser
{
    public class CountdownController : MonoBehaviour
    {
        [SerializeField] private GameObject countdownOverlay;
        [SerializeField] private TMP_Text counterText;

        public IEnumerator StartCountdown(int n)
        {
            countdownOverlay.SetActive(false);
            yield return StartCoroutine(CountdownRoutine(n));
        }

        private IEnumerator CountdownRoutine(int n)
        {
            for (int i = n; i > 0; i--)
            {
                counterText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            // 시작!
            counterText.text = "Start!";
            yield return new WaitForSeconds(1f);

            // 카운트다운 오버레이 숨기기
            countdownOverlay.SetActive(false);
        }

    }
}