using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class ResultPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Image mImage;
        [SerializeField] private UnityEvent onExitClicked;
        [SerializeField] private Sprite successImage;
        [SerializeField] private Sprite failureImage;

        public void ShowSuccess(string title = "success!", string desc = "get hint")
        {
            panel.SetActive(true);
            mImage.sprite = successImage; 
        }

        public void ShowFail(string title = "fail...", string desc = "miluna mad")
        {
            panel.SetActive(true);
            mImage.sprite = failureImage;
        }

        public void OnClickExit() => onExitClicked?.Invoke();
        public void Hide() => panel.SetActive(false);
    }
}