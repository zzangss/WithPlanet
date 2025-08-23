using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TalkManager talkManager;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI talkText;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private GameObject scanObject;

    public bool isAction = false;
    public int talkIndex;

    public void Action (GameObject scanObj)
    {
        scanObject = scanObj;
        ObjData objData = scanObj.GetComponent<ObjData>();
        npcName.text = objData.name;
        Talk(objData.id, objData.isNpc);

        dialoguePanel.SetActive(isAction);
        PauseController.SetPause(isAction);
    }

    public void Talk(int id, bool isNpc)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);

        if(talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            return;
        }
        if (isNpc)
        {
            talkText.text = talkData;
        }
        else
        {
            talkText.text = talkData;
        }

        isAction = true;
        talkIndex++;
    }
}
