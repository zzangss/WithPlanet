using System.Collections.Generic;
using UnityEngine;

// PlayerController: 2D ĳ���͸� ����Ű �Է����� �����̰� ���ִ� ��ũ��Ʈ
public class PlayerController : MonoBehaviour
{
    // �̵� �ӵ��� �����ϴ� ���� (Inspector���� ���� ����)
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private float runMultiplier = 1.8f;

    private Vector3 movement = new Vector3();

    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // GetAxisRaw�� Ű���� �Է��� -1, 0, 1�� ��ȯ (�߰� �� ����)
        movement.x = Input.GetAxisRaw("Horizontal");  // A/D or ��/��
        movement.z = Input.GetAxisRaw("Vertical");    // W/S or ��/��

    }

    void FixedUpdate()
    {
        // �밢�� �̵� �� �ӵ��� �ʹ� �������� �� ���� ���� ����ȭ
        movement.Normalize();

        // ���� �Ӽ��� Rigidbody�� velocity�� ���� �����Ͽ� �̵�
        // �ӵ� = ���� * �ӷ� * �޸�����
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        rigidbody.velocity = movement * movementSpeed * (isRunning ? runMultiplier : 1f);

        rigidbody.freezeRotation = true;
    }

}