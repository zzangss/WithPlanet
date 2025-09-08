using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InteractPromptUI : MonoBehaviour
{
    [Header("표시할 텍스트 UI")]
    [SerializeField] private TMP_Text uiText; 

    [Header("페이드/표시 제어")]
    [SerializeField] private CanvasGroup group;

    void Awake()
    {
        if (group == null) group = GetComponent<CanvasGroup>();
        Hide();
    }

    public void Show(List<KeyCode> keys, List<string> actions, string targetName = "")
    {
        uiText.text = null;
        for (int i = 0; i < keys.Count; i++) 
        { 
            if (uiText != null)
            {
                uiText.text += $"Press [{keys[i]}] {actions[i]} \n";
            }
            if (group != null) 
            { 
                group.alpha = 1f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
            else gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (group != null) 
        { 
            group.alpha = 0f; 
            group.interactable = false; 
            group.blocksRaycasts = false; 
        }
        else gameObject.SetActive(false);
    }
}
