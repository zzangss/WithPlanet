using UnityEngine;

public class TotalValueCounter : MonoBehaviour
{
    [SerializeField] private InventoryMain inventory; // 인스펙터로 할당
    private int lastTotal = -1;
    private int realTotal = 0;
    private int total = 0;

    private void Awake()
    {
        Debug.Log($"[CVC] Awake on {gameObject.name} (scene={gameObject.scene.name})");
    }

    private void OnEnable()
    {
        Debug.Log($"[CVC] OnEnable enabled={enabled}, activeInHierarchy={gameObject.activeInHierarchy}");
        TryWire();
    }

    private void Start()
    {
        Debug.Log("[CVC] Start");
        // 시작 시 한 번 계산 (씬 전환 직후 상태 확인용)
        RecomputeTotal(forceLog: true);
    }

    private void OnDisable()
    {
        Debug.Log("[CVC] OnDisable");
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= HandleInventoryChanged;
        }
    }

    private void TryWire()
    {
        if (inventory == null)
        {
            inventory = FindObjectOfType<InventoryMain>(true); // 비활성 오브젝트까지 검색
            Debug.Log($"[CVC] wiring inventory={(inventory ? inventory.name : "NULL")}");
        }

        if (inventory != null)
        {
            inventory.OnInventoryChanged -= HandleInventoryChanged; // 중복 방지
            inventory.OnInventoryChanged += HandleInventoryChanged;
            Debug.Log("[CVC] Subscribed to OnInventoryChanged");
        }
        else
        {
            Debug.LogWarning("[CVC] InventoryMain not found. Will retry in Update.");
        }
    }

    private void Update()
    {
        // 처음 프레임들에 참조가 아직 안 잡힌 경우 대비 재시도
        if (inventory == null && Time.frameCount < 300)
        {
            TryWire();
        }
    }

    private void HandleInventoryChanged()
    {
        RecomputeTotal();
    }

    private void RecomputeTotal(bool forceLog = false)
    {
        if (inventory == null)
        {
            Debug.LogWarning("[CVC] RecomputeTotal called but inventory is NULL");
            return;
        }

        var slots = inventory.GetAllItems();
        if (slots == null)
        {
            Debug.LogWarning("[CVC] slots is NULL");
            return;
        }

        int total = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            var s = slots[i];
            if (s != null && s.Item != null)
            {
                int count = s.mItemCount; // 프로젝트에 맞게 수정
                int value = s.Item.Value; // 프로젝트에 맞게 수정
                if(s.Item.Type == ItemType.OHTER)
                {
                    realTotal += -(value * count);
                    continue;
                }
                total += value * count;
                realTotal += value * count;
            }
        }

        // 값이 바뀌었을 때만 로그
        if (forceLog || total != lastTotal)
        {
            Debug.Log($"[CVC] TOTAL VALUE = {total} (frame={Time.frameCount}, time={Time.time:F2})");
            lastTotal = total;
        }
    }
}
