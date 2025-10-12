using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueClickCatcher : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.Next();
    }
}
