using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [Header("표시용 텍스트")]
    [SerializeField] private List<string> actionTexts = new List<string>();
    [SerializeField] private List<KeyCode> interactKeys = new List<KeyCode>();

    public List<string> ActionText
    {
        get { return actionTexts; }
    }
    public List<KeyCode> InteractKey
    {
        get { return interactKeys; }
    }

    void Reset()
    {
        // 트리거로 설정해 플레이어가 겹칠 수 있게
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }
}
