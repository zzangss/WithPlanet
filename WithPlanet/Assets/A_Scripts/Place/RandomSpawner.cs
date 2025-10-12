using UnityEngine;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    [Header("아이템 설정")]
    public List<SpawnItem> spawnItems = new List<SpawnItem>();

    [Header("스폰 범위")]
    public Vector3 spawnAreaSize = new Vector3(10, 10, 10); // Y값은 레이캐스트 탐지 거리로 활용됩니다.

    [Header("스폰 조건")]
    public float minDistance = 2f;
    public LayerMask groundLayer; // "Ground" 레이어를 지정할 변수

    private List<Vector3> spawnedPositions = new List<Vector3>();

    /// <summary>
    /// 아이템 스폰을 시작하는 함수
    /// </summary>
    public void SpawnItems()
    {
        // 이미 생성된 아이템 위치 기록 초기화
        spawnedPositions.Clear();

        foreach (var item in spawnItems)
        {
            if (item.itemPrefab == null) continue;

            // 스폰 확률을 적용하여 이번에 생성할 개수 결정
            int spawnTarget = Mathf.RoundToInt(item.count * item.spawnProbability);

            for (int i = 0; i < spawnTarget; i++)
            {
                // 유효한 스폰 위치를 찾음
                Vector3? randomPos = GetValidPosition();

                if (randomPos.HasValue)
                {
                    // 위치를 찾았으면 아이템을 생성하고 위치를 기록
                    Instantiate(item.itemPrefab, randomPos.Value, item.itemPrefab.transform.rotation);
                    spawnedPositions.Add(randomPos.Value);
                }
                else
                {
                    // 100번 시도 후에도 유효한 위치를 못 찾았으면 경고 출력 후 중단
                    Debug.LogWarning($"아이템 '{item.itemPrefab.name}'의 유효한 스폰 위치를 찾을 수 없습니다. 스폰 범위를 늘리거나 최소 거리를 줄여보세요.");
                    // 이 경우, 현재 아이템의 스폰 시도를 중단하고 다음 아이템으로 넘어갈 수 있습니다.
                    // 만약 실패해도 계속 시도하고 싶다면 아래 break;를 주석 처리하고 i--; 를 활성화하세요.
                    break;
                    // i--; // 실패했으니 다시 시도 (무한 루프의 위험이 있어 권장하지 않음)
                }
            }
        }
    }

    /// <summary>
    /// Raycast를 사용하여 Ground 레이어 위에 유효한 스폰 위치를 찾는 함수
    /// </summary>
    /// <returns>유효한 위치를 찾으면 Vector3, 못 찾으면 null</returns>
    Vector3? GetValidPosition()
    {
        // 최대 100번까지 유효한 위치를 찾기 위해 시도
        for (int attempt = 0; attempt < 100; attempt++)
        {
            // 1. 스폰 영역 내에서 랜덤한 x, z 좌표 계산
            float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float z = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);

            // 2. Raycast를 시작할 위치 설정 (스폰 영역의 맨 위)
            Vector3 rayOrigin = transform.position + new Vector3(x, spawnAreaSize.y / 2f, z);

            // 3. 아래 방향으로 Raycast 발사
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, spawnAreaSize.y, groundLayer))
            {
                // Raycast가 Ground 레이어에 닿았다면, 그 위치가 후보 위치가 됨
                Vector3 candidate = hit.point;

                // 4. 기존에 스폰된 아이템들과의 최소 거리 확인
                bool tooClose = false;
                foreach (Vector3 pos in spawnedPositions)
                {
                    if (Vector3.Distance(pos, candidate) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                // 5. 너무 가깝지 않으면 유효한 위치이므로 반환
                if (!tooClose)
                    return candidate;
            }
        }

        // 100번 시도해도 유효한 위치를 찾지 못하면 null 반환
        return null;
    }

    /// <summary>
    /// 유니티 에디터에서 스폰 범위를 시각적으로 보여주는 Gizmo
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, spawnAreaSize);
    }
}

[System.Serializable]
public class SpawnItem
{
    public GameObject itemPrefab;
    public int count;
    [Range(0f, 1f)]
    public float spawnProbability = 1f;
}