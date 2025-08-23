using UnityEngine;
using TMPro;

// 미니게임 매니저의 static 이벤트를 구독할 예정(있다면)
using Project.Minigames.ToxicCleanser;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TalkManager talkManager;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI talkText;
    [SerializeField] private TextMeshProUGUI npcNameText;

    [Header("NPC Objects")]
    [SerializeField] private ObjData bossNpc;       
    [SerializeField] private ObjData milunaNpc;
    [SerializeField] private ObjData boxMonsterNpc;

    // 상태
    public bool isAction = false;
    public int talkIndex = 0;

    // 전역 대화 단계 플래그
    public State CurrentStage { get; private set; } = State.Opening;
    private bool openingPlayed = false;
    private bool minigameFinished = false;
    private bool minigameSuccess = false;

    private ObjData currentNpc; // 현재 대화중인 NPC

    private void OnEnable()
    {
        // 미니게임 결과 이벤트 구독(가지고 계신 매니저가 있다면)
        ToxicCleanserMinigameManager.OnMinigameSuccess += HandleMinigameSuccess;
        ToxicCleanserMinigameManager.OnMinigameFail += HandleMinigameFail;
    }

    private void OnDisable()
    {
        ToxicCleanserMinigameManager.OnMinigameSuccess -= HandleMinigameSuccess;
        ToxicCleanserMinigameManager.OnMinigameFail -= HandleMinigameFail;
    }

    public void StartTutorial()
    {
        if (!openingPlayed && bossNpc != null)
        {
            StartOpeningDialogue();
        }
        else
        {
            // 혹시 오프닝을 스킵한다면, 미니게임 전 단계로 전환
            SetStage(State.PreMinigame);
        }
    }

    // 외부(플레이어 상호작용)에서 호출: PlayerAction에서 Space 시 Action(scanObj)
    public void Action(Type scanObjType)
    {
        if (scanObjType == bossNpc.type) 
        { 
            currentNpc = bossNpc;
        }
        else if(scanObjType == milunaNpc.type)
        {
            currentNpc = milunaNpc;
        }
        else if(scanObjType == boxMonsterNpc.type)
        {
            currentNpc = boxMonsterNpc;
        }

        npcNameText.text = currentNpc.name;
        portraitImage.sprite = currentNpc.portrait;
        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);

        Talk(currentNpc.type);
    }

    // 현재 스테이지 기준으로 해당 NPC의 대사 1줄 재생
    public void Talk(Type npcId)
    {
        string line = talkManager.GetTalk(npcId, CurrentStage, talkIndex);

        if (line == null)
        {
            // 대사 끝
            isAction = false;
            dialoguePanel.SetActive(false);
            PauseController.SetPause(false);
            talkIndex = 0;

            // 오프닝이 끝났다면 다음 단계로 전환
            if (CurrentStage == State.Opening)
            {
                openingPlayed = true;
                SetStage(State.PreMinigame);
            }
            return;
        }

        talkText.text = line;
        isAction = true;
        talkIndex++;
    }

    // Boss 오프닝을 강제로 시작
    public void StartOpeningDialogue()
    {
        SetStage(State.Opening);
        talkIndex = 0;

        // Boss와 바로 상호작용을 시작한 것처럼 처리
        currentNpc = bossNpc;
        npcNameText.text = bossNpc.name;
        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);
        Talk(bossNpc.type);
    }

    // 외부에서 단계 변경이 필요할 때 호출
    public void SetStage(State next)
    {
        CurrentStage = next;
        // 필요하다면 이 시점에 UI, 퀘스트, 트리거 등을 같이 전환
    }

    // 미니게임 결과 연동
    private void HandleMinigameSuccess()
    {
        minigameFinished = true;
        minigameSuccess = true;
        SetStage(State.PostMinigame);
    }

    private void HandleMinigameFail()
    {
        minigameFinished = true;
        minigameSuccess = false;
        SetStage(State.PostMinigame);
    }
}
