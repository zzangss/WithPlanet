using System.Collections.Generic;
using UnityEngine;

namespace Project.Minigames.ToxicCleanser
{
    [CreateAssetMenu(menuName = "Minigame2/Image Set", fileName = "ImageSet_MG2")]
    public class ImageSet : ScriptableObject
    {
        [Header("Gameplay")]
        [Min(1)] public int hearts = 3;                 // 초기 하트 수
        [Min(5f)] public float totalDurationSec = 30f;  // 전체 제한 시간(초)

        [Tooltip("웨이브(그리드) 리프레시 주기(초)")]
        [Min(0.3f)] public float refreshInterval = 1.5f;

        [Tooltip("그리드 내 제거 대상(타깃) 비율(0~1)")]
        [Range(0f, 1f)] public float targetSpawnRatio = 0.4f;

        [Header("Assets")]
        [Tooltip("클릭해 제거해야 하는 이미지들(타깃)")]
        public List<Sprite> targetSprites = new List<Sprite>();

        [Tooltip("클릭하면 안 되는 이미지들(중립)")]
        public List<Sprite> neutralSprites = new List<Sprite>();
    }
}
