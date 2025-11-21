using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Tooltip("아이템 데이터")]
    public ItemData data;
    [Header("연출 설정")]
    public AudioClip pickupSound; // 획득 효과음
    public GameObject pickupEffectPrefab; // 획득 파티클 (선택)

    private bool isCollected = false; // 중복 획득 방지 플래그

    public void ItemPickuped()
    {
        if (isCollected) return;
        if (data == null)
        {
            Debug.LogError("ItemPickup에 ItemData가 연결되지 않았습니다!", this);
            return;
        }

        isCollected = true; // 획득 상태로 변경

        //  ItemManager에 획득 사실 통보
        ItemManager.Instance.CollectItem(data);

        StartCoroutine(ProcessPickupRoutine());

    }

    private IEnumerator ProcessPickupRoutine()
    {
        
        GetComponent<Renderer>().enabled = false;   // 눈에 안 보임 (3D면 MeshRenderer, 2D면 SpriteRenderer)

        // [사운드 처리] 소리가 있다면 재생
        if (pickupSound != null)
        {
            // 방법 A: AudioSource가 이 오브젝트에 있다면 재생하고 기다림
            AudioSource audio = GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.PlayOneShot(pickupSound);
                yield return new WaitForSeconds(pickupSound.length); // 소리 길이만큼 대기
            }
            else
            {
                // 방법 B: 이 오브젝트가 사라져도 소리가 나게 하려면 (추천)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
        }

        // [이펙트 처리] 파티클이 있다면 생성
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }

        // (선택) 만약 별도의 획득 애니메이션이 있다면 여기서 yield return new WaitForSeconds(...) 로 대기

        // 4. 모든 연출이 끝났으니 진짜 삭제
        Destroy(gameObject);
    }
}
