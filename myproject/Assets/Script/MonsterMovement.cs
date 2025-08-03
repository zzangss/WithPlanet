using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MonsterMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f; //�̵��ӵ�
    public float patrolDistance = 5f; //�����Ÿ�
    private Vector3 startPosition;
    private int direction = 1; //�̵�����
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; //������ġ ����
    }
    // Update is called once per frame
    void Update()
    {
        if (Turn())
        {
            Flip();
        }
        MoveMonster();
    }
    void MoveMonster()
    {
        transform.Translate(Vector3.right * moveSpeed * direction * Time.deltaTime);
    }
    bool Turn()
    {
        float distanceFromStart = transform.position.x - startPosition.x;

        if (direction == 1 && distanceFromStart >= patrolDistance)
        {
            return true; //������ ���� ����
        }
        else if (direction == -1 && distanceFromStart <= -patrolDistance)
        {
            return true; //���� ���� ����
        }
        return false;
    }
    void Flip()
    {
        direction *= -1; //���� ����

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = !sr.flipX; //��������Ʈ ���� ����
        }
    }
    void OnDrawGizmos()
    {
        Vector3 currentStart = Application.isPlaying ? startPosition : transform.position;
        Vector3 drawPosition = new Vector3(currentStart.x, currentStart.y - 3f, currentStart.z);

        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Vector3 boxSize = new Vector3(patrolDistance * 2, 1f, 1f);
        Gizmos.DrawCube(drawPosition, boxSize);
    }
}