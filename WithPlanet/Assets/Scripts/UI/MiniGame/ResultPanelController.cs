using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Minigames.ToxicCleanser
{
    public class ResultPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private UnityEvent onExitClicked;



        public void ShowSuccess(string title = "¼º°ø!", string desc = "º¸¹° ÈùÆ®¸¦ È¹µæÇß½À´Ï´Ù.")
        {
            panel.SetActive(true);
            titleText.text = title;
            descText.text = desc;
        }

        public void ShowFail(string title = "½ÇÆÐ...", string desc = "¹Ì·ç³ª°¡ ±¤±â¿¡ ÈÛ½Î¿´½À´Ï´Ù.")
        {
            panel.SetActive(true);
            titleText.text = title;
            descText.text = desc;
        }

        public void OnClickExit() => onExitClicked?.Invoke();
        public void Hide() => panel.SetActive(false);
    }
}