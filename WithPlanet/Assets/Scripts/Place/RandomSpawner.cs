using UnityEngine;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    [Header("아이템 설정")]
    public List<SpawnItem> spawnItems = new List<SpawnItem>();

    [Header("스폰 범위")]
    public Vector3 spawnAreaSize = new Vector3(10, 1, 10);

    [Header("스폰 조건")]
    public float minDistance = 2f;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        foreach (var item in spawnItems)
        {
            if (item.itemPrefab == null) continue;

            int spawnTarget = Mathf.RoundToInt(item.count * item.spawnProbability);

            for (int i = 0; i < spawnTarget; i++)
            {
                Vector3? randomPos = GetValidPosition();

                if (randomPos == null)
                {
                    i--; // 실패했으니 다시 시도
                    continue;
                }

                Instantiate(item.itemPrefab, randomPos.Value, Quaternion.identity);
                spawnedPositions.Add(randomPos.Value);
            }
        }
    }

    Vector3? GetValidPosition()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float z = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);
            float y = transform.position.y; // y포지션 뭘 가져오는거지?


            Vector3 candidate = new Vector3(x, y, z) + transform.position;

            bool tooClose = false;
            foreach (Vector3 pos in spawnedPositions)
            {
                if (Vector3.Distance(pos, candidate) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return candidate;
        }

        return null;
    }

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
