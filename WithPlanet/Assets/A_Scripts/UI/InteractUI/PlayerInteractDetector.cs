// PlayerInteractDetector.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerInteractDetector : MonoBehaviour
{
    [SerializeField] private InteractPromptUI promptUI;

    private Interactable current;

    void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private Interactable FindInteractable(Component c)
    {
        if (c == null) return null;
        // 자식/부모 어디에 붙어있든 찾아보기
        return c.GetComponent<Interactable>()
            ?? c.GetComponentInParent<Interactable>()
            ?? c.GetComponentInChildren<Interactable>();
    }

    void OnTriggerEnter(Collider other)
    {
        var interactable = FindInteractable(other);
        if (interactable == null) return;

        current = interactable;
        if (promptUI != null)
        {
            promptUI.Show(interactable.InteractKey, interactable.ActionText, other.gameObject.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (current != null && FindInteractable(other) == current)
        {
            current = null;
            promptUI?.Hide();
        }
    }
}
