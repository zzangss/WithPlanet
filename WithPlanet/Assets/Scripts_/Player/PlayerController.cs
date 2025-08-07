using System.Collections.Generic;
using UnityEngine;

// PlayerController: 2D 캐릭터를 방향키 입력으로 움직이게 해주는 스크립트
public class PlayerController : MonoBehaviour
{
    // 이동 속도를 조절하는 변수 (Inspector에서 설정 가능)
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
        // GetAxisRaw는 키보드 입력을 -1, 0, 1로 반환 (중간 값 없음)
        movement.x = Input.GetAxisRaw("Horizontal");  // A/D or ←/→
        movement.z = Input.GetAxisRaw("Vertical");    // W/S or ↑/↓

    }

    void FixedUpdate()
    {
        // 대각선 이동 시 속도가 너무 빨라지는 걸 막기 위해 정규화
        movement.Normalize();

        // 물리 속성인 Rigidbody의 velocity를 직접 설정하여 이동
        // 속도 = 방향 * 속력 * 달리기배수
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        rigidbody.velocity = movement * movementSpeed * (isRunning ? runMultiplier : 1f);

        rigidbody.freezeRotation = true;
    }

}