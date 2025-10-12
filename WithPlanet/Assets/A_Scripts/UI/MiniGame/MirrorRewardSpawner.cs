using UnityEngine;
using Project.Minigames.ToxicCleanser; // ToxicCleanserMinigameManager 이벤트

/// <summary>
/// 미니게임 '성공' 시 거울 아이템을 월드에 생성.
/// - Item SO가 있으면 DropItemToWorld 사용
/// - SO가 없고 ID만 있으면 spawnItemToWorld 사용
/// </summary>
public class MirrorRewardSpawner : MonoBehaviour
{
    [Header("Spawner Reference")]
    [SerializeField] private ItemSpawner itemSpawner;   // 씬의 ItemSpawner 참조

    [Header("Mirror Item (둘 중 하나 사용)")]
    [SerializeField] private Item mirrorItemSO;         // ScriptableObject (권장)

    [Header("Spawn Options")]
    [SerializeField] private bool spawnOnce = true;     // 한 번만 스폰할지
    [SerializeField] private int spawnCount = 1;        // 생성 개수 

    private bool _spawned = false;

    private void Reset()
    {
        if (itemSpawner == null)
            itemSpawner = FindObjectOfType<ItemSpawner>();
    }

    private void OnEnable()
    {
        ToxicCleanserMinigameManager.OnMinigameSuccess += HandleSuccess;
    }

    private void OnDisable()
    {
        ToxicCleanserMinigameManager.OnMinigameSuccess -= HandleSuccess;
    }

    private void HandleSuccess()
    {
        if (spawnOnce && _spawned) return;
        if (itemSpawner == null)
        {
            Debug.LogWarning("[MirrorRewardSpawner] ItemSpawner reference is missing.");
            return;
        }

        // 1) Item SO가 있으면 DropItemToWorld 사용
        if (mirrorItemSO != null)
        {
            // ItemSpawner.DropItemToWorld는 내부에서 플레이어 앞 위치로 산포
            itemSpawner.DropItemToWorld(mirrorItemSO, Mathf.Max(1, spawnCount));
        }
        else
        {
            Debug.LogWarning("[MirrorRewardSpawner] No mirror item assigned");
            return;
        }

        _spawned = true;
    }
}
