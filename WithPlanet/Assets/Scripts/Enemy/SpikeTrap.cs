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
        upPosition = downPosition + new Vector3(0, 1, 0); // ���� �̵�

        StartCoroutine(SpikeRountine());

    }

    IEnumerator SpikeRountine()
    {
        while (true)
        {
            //�ö󰡱�
            yield return StartCoroutine(MoveTo(upPosition));
            isUp = true;

            //�ö� ���� ����
            yield return new WaitForSeconds(upTime);

            //��������
            yield return StartCoroutine(MoveTo(downPosition));
            isUp = false;

            //���
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
        transform.position = target; // ��Ȯ�� ��ġ ���߱�
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isUp)
        {
            // �÷��̾ ������ũ�� ����� �� ó��
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20f, transform.position, 15f);
            }
        }
    }




}
