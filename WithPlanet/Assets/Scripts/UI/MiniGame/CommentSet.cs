using System.Collections.Generic;
using UnityEngine;

namespace Project.Minigames.ToxicCleanser
{
    [CreateAssetMenu(menuName = "Minigame/ToxicCleanser/CommentSet", fileName = "CommentSet")]
    public class CommentSet : ScriptableObject
    {
        public List<CommentData> untoxicComments = new();
        public List<CommentData> toxicComments = new();
        [Range(0f, 1f)] public float toxicSpawnRatio = 0.4f;
        [Min(50f)] public float scrollSpeed = 250f; // px/sec
        [Min(1)] public int totalDurationSec = 50;
        [Min(1)] public int hearts = 4;
    }
}