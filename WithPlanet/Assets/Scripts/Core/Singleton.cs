using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour�� ��ӹ��� Ŭ������ ���׸� ������� �̱��� ����� �����Ѵ�.
/// </summary>
/// <typeparam name="T">�̱������� ����� ���� Ÿ�� (MonoBehaviour�� ����ؾ� ��)</typeparam>

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T _instance;

    public static T Instance// �̱��� �ν��Ͻ� ���� �� ��� 
    {
        get
        {
            if(_instance == null) // ���� ������ �ν��Ͻ��� ������ ������ ã�Ƽ� �Ҵ� 
            {
                _instance = FindObjectOfType<T>(); // O(N) �̹Ƿ� ������ ��� 
            }
            return _instance; 
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null) // ������ �ν��Ͻ� ���� -> ���� ȣ��
        {
            _instance = this as T; // �ڽ��� Ÿ��(T)�� ĳ�����Ͽ� ����. 
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // �̹� �ٸ� �ν��Ͻ��� ������ �ڽ��� destroy 
        }
    }
}
