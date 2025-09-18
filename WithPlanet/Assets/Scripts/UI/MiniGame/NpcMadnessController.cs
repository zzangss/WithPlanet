using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Project.Minigames.ToxicCleanser; // 네가 말한 매니저 네임스페이스

public class NPCMadnessController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NaviMonster naviMonster;
    [SerializeField] private Transform player;
    [SerializeField] private MonsterAttack monsterAttack;

    [Header("Madness Settings")]
    [SerializeField] private float madnessDuration = 10f; // 광폭화 지속 시간
    [SerializeField] private float attackRange = 1.6f; // 공격 범위
    [SerializeField] private float dps = 5f; // 초당 피해량

    [Header("Navi Override While Mad")]
    [SerializeField] private float madChaseSpeed = 10f; // 광폭화 시 추적 속도
    [SerializeField] private float madDetectionRange = 999f; // 광폭화 시 탐지 범위
    [SerializeField] private float madMaxChaseRange = 999f; // 광폭화 시 최대 추적 범위

    [Header("UI & Interaction Lock")]
    [SerializeField] private CanvasGroup attackOverlay; 
    [SerializeField] private Canvas attackOverlayCanvas; 
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();

    private bool isMad = false;

    private List<GraphicRaycaster> cachedRaycasters = new List<GraphicRaycaster>();

    private void OnEnable()
    {
        // 여기서 이벤트에 구독
        ToxicCleanserMinigameManager.OnMinigameFail += TryStartMadness;
        Minigame2Manager.OnMinigameFail += TryStartMadness; 
    }

    private void OnDisable()
    {
        ToxicCleanserMinigameManager.OnMinigameFail -= TryStartMadness;
        Minigame2Manager.OnMinigameFail -= TryStartMadness;
    }

    private void Awake()
    {
        if (naviMonster == null)
        {
            naviMonster = GetComponent<NaviMonster>();
        }

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (monsterAttack == null)
        {
            monsterAttack = GetComponent<MonsterAttack>();
        }
    }

    private void Start()
    {
        CacheRaycasters();
        HideAttackOverlayImmediate();
        monsterAttack.attackEnabled = false; // 처음에는 공격 비활성화
    }

    /// <summary>
    /// OnMinigameFail 이벤트에 붙일 구독자 메서드
    /// </summary>
    private void TryStartMadness()
    {
        if (isMad || naviMonster == null || player == null) return;
        StartCoroutine(MadnessRoutine());
    }

    private IEnumerator MadnessRoutine()
    {
        isMad = true;

        StartNavi();
        monsterAttack.attackEnabled = true; // 공격 스크립트 켜기

        LockInteractions(true);
        ShowAttackOverlay(true);

        float timer = 0f;
        while (timer < madnessDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ShowAttackOverlay(false);
        LockInteractions(false);
        StopNavi();

        monsterAttack.attackEnabled = false;
        isMad = false;
    }

    private void StartNavi()
    {
        naviMonster.chaseSpeed = madChaseSpeed;
        naviMonster.detectionRange = madDetectionRange;
        naviMonster.maxChaseRange = madMaxChaseRange;
    }

    private void StopNavi()
    {
        naviMonster.chaseSpeed = 0f;
        naviMonster.detectionRange = 0f;
        naviMonster.maxChaseRange = 0f;
    }

    // ---------------- UI 잠금 ----------------
    private void CacheRaycasters()
    {
        cachedRaycasters.Clear();
        cachedRaycasters.AddRange(FindObjectsOfType<GraphicRaycaster>(includeInactive: true));
    }

    private void LockInteractions(bool locked)
    {
        foreach (var b in componentsToDisable)
        {
            if (b != null) b.enabled = !locked;
        }

        foreach (var gr in cachedRaycasters)
        {
            if (gr == null) continue;
            if (attackOverlayCanvas != null && gr.gameObject == attackOverlayCanvas.gameObject)
                gr.enabled = true;
            else
                gr.enabled = !locked;
        }
    }

    private void ShowAttackOverlay(bool show)
    {
        if (attackOverlay == null) return;
        StopCoroutine(nameof(FadeAttackOverlay));
        StartCoroutine(FadeAttackOverlay(show ? 1f : 0f, 0.2f));
    }

    private void HideAttackOverlayImmediate()
    {
        if (attackOverlay == null) return;
        attackOverlay.alpha = 0f;
        attackOverlay.blocksRaycasts = false;
        attackOverlay.interactable = false;
    }

    private IEnumerator FadeAttackOverlay(float targetAlpha, float duration)
    {
        if (attackOverlay == null) yield break;

        float start = attackOverlay.alpha;
        float t = 0f;

        bool enable = targetAlpha > 0.5f;
        attackOverlay.blocksRaycasts = enable;
        attackOverlay.interactable = enable;

        while (t < duration)
        {
            t += Time.deltaTime;
            attackOverlay.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }
        attackOverlay.alpha = targetAlpha;
    }
}
