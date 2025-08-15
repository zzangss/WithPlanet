using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class HeartsView : MonoBehaviour
    {
        [SerializeField] private List<Image> hearts;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        public void SetHearts(int current, int max)
        {
            for (int i = 0; i < hearts.Count; i++)
            {
                if (i < max)
                {
                    hearts[i].enabled = true;
                    hearts[i].sprite = i < current ? onSprite : offSprite;
                }
                else {
                    hearts[i].enabled = false;
                 }
            }
        }
    }
}