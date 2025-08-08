using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMonsterAttack : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackCooldown = 2f;
    public GameObject bulletPrefab; // �Ѿ� ������
    public float bulletSpeed = 10f;

    private float lastAttackTime = 0f;
    private MonsterMovement monsterMovement; // ���� ������


    void Update()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Fire();
            lastAttackTime = Time.time;
        }
    }

    void Fire()
    {
        Vector3 spawnPosition = transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("�Ѿ� ������");
    }
}
