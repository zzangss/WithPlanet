using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Project.Minigames.ToxicCleanser;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("Refs")]
    [SerializeField] private TalkManager talkManager;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI talkText;
    [SerializeField] private TextMeshProUGUI npcNameText;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private TextMeshProUGUI choicePrompt;
    [SerializeField] private Button btnO;
    [SerializeField] private Button btnX;
    [SerializeField] private TextMeshProUGUI btnOText;
    [SerializeField] private TextMeshProUGUI btnXText;

    [Header("NPC Objects")]
    [SerializeField] private ObjData bossNpc;
    [SerializeField] private ObjData milunaNpc;
    [SerializeField] private ObjData boxMonsterNpc;

    public bool isAction = false;
    public int talkIndex = 0;

    public State CurrentStage { get; private set; } = State.Opening;
    private bool openingPlayed = false;

    public ObjData currentNpc;

    private DialogueEntry _pendingChoice;

    private void Awake()
    {
        if (btnO) btnO.onClick.AddListener(() => OnChoiceSelected(true));
        if (btnX) btnX.onClick.AddListener(() => OnChoiceSelected(false));
        ShowChoice(false);
    }

    public void Init()
    {
        dialoguePanel.SetActive(false);
        isAction = false;
        talkIndex = 0;
        openingPlayed = false;
        CurrentStage = State.Opening;
        ShowChoice(false);
    }

    private void OnEnable()
    {
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
        if (!openingPlayed && bossNpc != null) StartOpeningDialogue();
        else SetStage(State.PreMinigame);
    }

    // 플레이어 상호작용에서 호출: 스캔된 NPC 타입 전달
    public void Action(Type scanObjType)
    {
        if (scanObjType == bossNpc.type) currentNpc = bossNpc;
        else if (scanObjType == milunaNpc.type) currentNpc = milunaNpc;
        else if (scanObjType == boxMonsterNpc.type) currentNpc = boxMonsterNpc;

        npcNameText.text = currentNpc.name;
        portraitImage.sprite = currentNpc.portrait; // ObjData에서 초상화 사용 :contentReference[oaicite:7]{index=7}
        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);
        Talk(currentNpc.type);
    }

    // 패널 아무 곳 클릭 → 다음 줄 (선택지 떠 있으면 무시)
    public void Next()
    {
        if (choicePanel != null && choicePanel.activeSelf) return;
        Talk(currentNpc.type);
    }

    public void Talk(Type npcId)
    {
        var entry = talkManager.GetTalk(npcId, CurrentStage, talkIndex);

        if (entry == null)
        {
            // 대사 종료 처리
            isAction = false;
            dialoguePanel.SetActive(false);
            PauseController.SetPause(false);
            talkIndex = 0;

            if (CurrentStage == State.Opening)
            {
                openingPlayed = true;
                SetStage(State.PreMinigame);
            }
            return;
        }

        switch (entry.kind)
        {
            case LineKind.Text:
                ShowChoice(false);
                talkText.text = entry.text;
                isAction = true;
                talkIndex++;
                break;

            case LineKind.Choice:
                SetupChoice(entry);
                isAction = true;
                break;
        }
    }

    private void SetupChoice(DialogueEntry entry)
    {
        talkText.text = "";
        if (choicePrompt) choicePrompt.text = entry.choicePrompt;
        if (btnOText) btnOText.text = string.IsNullOrEmpty(entry.optionO) ? "O" : entry.optionO;
        if (btnXText) btnXText.text = string.IsNullOrEmpty(entry.optionX) ? "X" : entry.optionX;
        _pendingChoice = entry;
        ShowChoice(true);
    }

    private void OnChoiceSelected(bool isO)
    {
        if (_pendingChoice == null) return;

        int next = talkIndex + 1;
        if (isO && _pendingChoice.nextIndexIfO >= 0) next = _pendingChoice.nextIndexIfO;
        if (!isO && _pendingChoice.nextIndexIfX >= 0) next = _pendingChoice.nextIndexIfX;

        // (원하면 여기서 선택 결과 플래그를 저장해 진행 상태에 반영 가능)
        // GameProgress.acceptedHelpFromMiluna = isO;  등

        talkIndex = next;
        _pendingChoice = null;
        ShowChoice(false);
        Talk(currentNpc.type);
    }

    private void ShowChoice(bool show)
    {
        if (choicePanel != null) choicePanel.SetActive(show);
    }

    public void StartOpeningDialogue()
    {
        SetStage(State.Opening);
        talkIndex = 0;
        currentNpc = bossNpc;
        npcNameText.text = bossNpc.name;
        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);
        Talk(bossNpc.type);
    }

    public void SetStage(State next) => CurrentStage = next;

    private void HandleMinigameSuccess() => SetStage(State.PostMinigame);
    private void HandleMinigameFail() => SetStage(State.PreMinigame);
}
