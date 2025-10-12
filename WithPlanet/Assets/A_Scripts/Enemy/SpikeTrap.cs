using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float cycleTime = 2f;
    public float upTime = 1f;
    public float speed = 5f;

    private Vector3 downPosition;
    private Vector3 upPosition;
    private bool isUp = false;


    // Start is called before the first frame update
    void Start()
    {
        downPosition= transform.position;
        upPosition = downPosition + new Vector3(0, 1, 0); // 위로 이동

        StartCoroutine(SpikeRountine());

    }

    IEnumerator SpikeRountine()
    {
        while (true)
        {
            //올라가기
            yield return StartCoroutine(MoveTo(upPosition));
            isUp = true;

            //올라간 상태 유지
            yield return new WaitForSeconds(upTime);

            //내려가기
            yield return StartCoroutine(MoveTo(downPosition));
            isUp = false;

            //대기
            yield return new WaitForSeconds(cycleTime - upTime);
        }
    }

    IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        { 
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target; // 정확히 위치 맞추기
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isUp)
        {
            // 플레이어가 스파이크에 닿았을 때 처리
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20f, transform.position, 15f);
            }
        }
    }




}
