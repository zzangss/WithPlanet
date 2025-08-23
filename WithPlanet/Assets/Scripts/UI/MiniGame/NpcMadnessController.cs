using System;
using System.Collections;
using UnityEngine;

namespace Project.Minigames.ToxicCleanser
{
    // 최소 의존성 버전: 외부 AI가 없어도 동작/컴파일 가능
    public class NpcMadnessController : MonoBehaviour
    {
        [Header("Target to modify (optional)")]
        [SerializeField] private MonoBehaviour aiLikeObject; // 외부 AI 스크립트를 참조해도 되고 비워도 됨
        [SerializeField] private float moveSpeed = 2f;   // 로컬 대체값
        [SerializeField] private float attackSpeed = 1f; // 로컬 대체값

        [Header("Player link (for damage)")]
        [SerializeField] private PlayerPoints playerPoints; // 점수/체력 관리자 (아래 간단 클래스)

        private float baseMove;
        private float baseAttack;
        private bool running;

        public void TriggerMadness(float duration, float speedMul, int damagePerHit)
        {
            if (!running) StartCoroutine(RunMadness(duration, speedMul, damagePerHit));
        }

        private IEnumerator RunMadness(float t, float mul, int dmg)
        {
            running = true;
            CacheBase();
            ApplyMul(mul);
            float time = 0f;
            while (time < t)
            {
                time += Time.deltaTime;
                // 예시: 주기적으로 플레이어 타격 이벤트 발생 (실게임에선 AI 이벤트에 연결)
                if (UnityEngine.Random.value < 0.02f && playerPoints != null)
                {
                    playerPoints.AddPoints(-dmg);
                }
                yield return null;
            }
            RestoreBase();
            running = false;
        }

        private void CacheBase()
        {
            baseMove = moveSpeed;
            baseAttack = attackSpeed;
            // 외부 AI 스크립트와 연동하려면 여기서 속성 반영
        }

        private void ApplyMul(float mul)
        {
            moveSpeed *= mul;
            attackSpeed *= mul;
            // 외부 AI에 속도 전달 로직 추가 가능
        }

        private void RestoreBase()
        {
            moveSpeed = baseMove;
            attackSpeed = baseAttack;
        }
    }

    // 간단한 점수/체력 대체 객체 (필요 시 본인 PlayerStats로 교체)
    public class PlayerPoints : MonoBehaviour
    {
        [SerializeField] private int points = 100;
        public void AddPoints(int delta)
        {
            points += delta;
            Debug.Log($"Player points: {points}");
        }
    }
}