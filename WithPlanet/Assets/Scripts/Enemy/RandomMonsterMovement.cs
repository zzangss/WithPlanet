using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMonsterMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f; //�̵��ӵ�
    public float patrolDistance = 5f; //�����Ÿ�

    [Header("Random settings")]
    public float minMoveTime = 1f; //�ּ��̵��ð�
    public float maxMoveTime = 3f; //�ִ��̵��ð�
    public float minStopTime = 0.5f; //�ּ������ð�
    public float maxStopTime = 2f; //�ִ������ð�

    private Vector3 startPosition;
    private int direction = 1; //�̵�����
    private bool isMoving = true; //�̵�����
    private float actionTimer = 0f; // �ൿŸ�̸�
    private float currentActionTime; //���� �ൿ ���� �ð�

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; //������ġ ����
        SetNewAction();
    }

    // Update is called once per frame
    void Update()
    {
        // Ÿ�̸� ������Ʈ
        actionTimer += Time.deltaTime;

        //�ൿ�� �����ٸ� ���ο� �ൿ ����
        if (actionTimer >= currentActionTime)
        {
            SetNewAction();
        }

        //��� üũ �� �̵�
        if (Turn())
        {
            Flip();
            SetNewAction();
        }
        if (isMoving)
        {
            MoveMonster();
        }

    }

    void SetNewAction()
    {
        actionTimer = 0f;

        isMoving = Random.Range(0f, 1f) > 0.5f; //50% Ȯ���� �̵� �Ǵ� ����

        if (isMoving)
        {
            currentActionTime = Random.Range(minMoveTime, maxMoveTime); //�̵��ð� ����

            if (Random.Range(0f, 1f) > 0.5f) //50% Ȯ���� ���� ��ȯ
            {
                Flip();
            }


        }
        else
        {
            currentActionTime = Random.Range(minStopTime, maxStopTime); //�����ð� ����
        }
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
