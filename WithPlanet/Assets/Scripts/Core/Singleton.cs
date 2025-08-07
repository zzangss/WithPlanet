using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour를 상속받은 클래스에 제네릭 방식으로 싱글톤 기능을 제공한다.
/// </summary>
/// <typeparam name="T">싱글톤으로 만들고 싶은 타입 (MonoBehaviour를 상속해야 함)</typeparam>

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T _instance;

    public static T Instance// 싱글톤 인스턴스 얻을 때 사용 
    {
        get
        {
            if(_instance == null) // 아직 생성된 인스턴스가 없으면 씬에서 찾아서 할당 
            {
                _instance = FindObjectOfType<T>(); // O(N) 이므로 신중히 사용 
            }
            return _instance; 
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null) // 생성된 인스턴스 없음 -> 최초 호출
        {
            _instance = this as T; // 자신의 타입(T)로 캐스팅하여 저장. 
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 이미 다른 인스턴스가 있으면 자신은 destroy 
        }
    }
}
