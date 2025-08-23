using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    [Serializable]
    public class CommentData
    {
        [TextArea(1, 3)] public string text;
        public Image profile;
        public bool isToxic;
    }
}