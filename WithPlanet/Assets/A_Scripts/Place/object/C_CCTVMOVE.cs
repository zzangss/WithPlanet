using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class CCTVMoveController : MonoBehaviour
{
    [Header("cctv들")]
    [SerializeField] private GameObject[] cctvs;

    [Header("랜덤 감시 설정")]
    [Tooltip("목표 지점에 도착한 후 대기하는 시간 (최소, 최대)")]
    public Vector2 patrolWaitTimeRange = new Vector2(1f, 3f);
    [SerializeField] private Transform[] randomTargets;

    public int currentIndex = 0;
    private Coroutine patrolCoroutine;

    void Awake()
    {

        // 시작 전 배열 길이 체크로 안정성 확보
        if (cctvs.Length != randomTargets.Length)
        {
            Debug.LogError("CCTV와 감시 목표의 개수가 일치하지 않습니다! 확인해주세요.");
            return; // 개수가 다르면 실행 중지
        }

        for (int i = 0; i < cctvs.Length; i++)
        {
            // 모든 CCTV를 일단 비활성화
            cctvOFF(i);
        }

        cctvON(0);
    }

    void Start()
    {
        StartPatrol();
    }


    IEnumerator PatrolRoutine()
    {
        if(randomTargets == null || randomTargets.Length == 0)
        {
            Debug.LogWarning("감시목표가없습니다!");
            yield break;
        }

        while(true)
        {
            //Debug.Log("목표설정");
            //1. 랜덤으로 목표 선택
            int randomIndex = Random.Range(0, randomTargets.Length);

            //2. 현재 해당 인덱스 비활성화
            cctvOFF(currentIndex);

            //3. 랜덤 인덱스 목표 활성화
            cctvON( randomIndex);
            //3-1. currentindex변경
            currentIndex = randomIndex;


            //4. 목표지점 대기
            float waitTime= Random.Range(patrolWaitTimeRange.x,patrolWaitTimeRange.y);
            yield return new WaitForSeconds(waitTime);


        }
    }

    void cctvOFF(int index)
    {
        if (index >= 0 && index < cctvs.Length)
        {
            cctvs[index].SetActive(false);
        }
    }

    

    void cctvON(int index)
    {
        if (index >= 0 && index < cctvs.Length)
        {
            cctvs[index].SetActive(true);
          
        }
          
    }

    void StartPatrol()
    {
       // Debug.Log("감시 시작! ");
        if (patrolCoroutine == null)
        {
            patrolCoroutine = StartCoroutine(PatrolRoutine());
        }
    }
    void StopPatrol()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
    }
   
}