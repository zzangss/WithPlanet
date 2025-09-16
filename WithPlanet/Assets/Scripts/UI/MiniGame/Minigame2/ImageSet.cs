using System.Collections.Generic;
using UnityEngine;

namespace Project.Minigames.ToxicCleanser
{
    [CreateAssetMenu(menuName = "Minigame2/Image Set", fileName = "ImageSet_MG2")]
    public class ImageSet : ScriptableObject
    {
        [Header("Gameplay")]
        [Min(5f)] public int totalDurationSec = 30;   // 전체 제한 시간
        [Min(1)] public int targetGoal = 30;    // 제한시간 내 제거해야 할 총 타깃 수
        [Min(0.3f)] public float refreshInterval = 1.5f; // 새로고침 주기(초)
        [Range(0f, 1f)] public float targetSpawnRatio = 0.4f; // 최초 타깃 비율 

        [Header("Wrong Click (Non-Target) Penalty")]
        [Tooltip("타깃이 아닌 이미지를 눌렀을 때 남은 시간에서 깎을 초")]
        [Min(0f)] public float penaltySecondsOnWrongClick = 1.0f;

        [Header("Mid Flip (Neutral -> Target)")]
        [Tooltip("후보 1개 기준 초당 변신 확률 p (예: 0.2)")]
        [Range(0f, 1f)] public float flipProbPerSecond = 0.2f;
        [Tooltip("변신 체크 주기(초)")]
        [Range(0.05f, 1f)] public float flipCheckInterval = 0.25f;
        [Tooltip("한 번의 체크에서 최대 변신 개수")]
        [Min(0)] public int maxFlipsPerTick = 1;

        [Tooltip("보드에 동시에 존재 가능한 타깃 상한(과도한 증가 방지). 0 또는 음수면 제한 없음")]
        public int maxTargetsOnBoard = 5;

        [Header("Assets")]
        public List<Sprite> targetSprites = new List<Sprite>();
        public List<Sprite> neutralSprites = new List<Sprite>();
    }
}
