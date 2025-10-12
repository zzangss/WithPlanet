using UnityEngine;
using UnityEngine.UI; // UI 관련 기능을 사용하기 위해 필수
using System; // Action 이벤트를 사용하기 위해 필요

/// <summary>
/// 게임의 전반적인 UI를 관리하고, 게임 이벤트에 반응하여 UI를 업데이트합니다.
/// GameManager와 SaveManager로부터 이벤트를 구독하여 UI를 제어합니다.
/// </summary>

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // UI �г� ����
    public GameObject menuPanel;
    public GameObject gamePanel; //인게임 UI 패널(체력바, 스테이지 등)
    public GameObject pausePanel;
    public GameObject savePanel;
    public GameObject settingsPanel;
    public DialogueManager dialogueManager;
    public GameObject gameOverPanel; // 게임 오버 패널 

    //[SerializeField] private GameManagerSystem gameManagerSystem;
    [SerializeField] private SaveSlotSelector saveSlotSelector;
    [SerializeField] private InventoryMain inventoryMain;

    [Header("In-Game UI")]
    [SerializeField] private Slider healthBarSlider; // 체력 바 UI (Slider)
    [SerializeField] private Text stageNumberText;  // 스테이지 번호를 표시할 UI Text

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton; // 새 게임 버튼
    [SerializeField] private Button loadGameButton; // 게임 불러오기 버튼 (SavePanel로 이동)
    [SerializeField] private Button saveGameInPausePanelButton; // 일시정지 패널 내 저장 버튼
    [SerializeField] private Button exitGameButton; // 게임 종료 버튼
    [SerializeField] private Button restartGameButton; // 게임 오버 패널 내 재시작 버튼
 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

      
    }

    void Start()
    {
        // ���� ���� �� �޴� �г� ����
        OpenMenuPanel();
        newGameButton?.onClick.AddListener(OnNewGameButtonClicked);
        loadGameButton?.onClick.AddListener(OpenSavePanel); // 메인 메뉴의 불러오기 버튼은 SavePanel을 엽니다.
        saveGameInPausePanelButton?.onClick.AddListener(OnSaveGameButtonClicked);
        exitGameButton?.onClick.AddListener(OnExitGameButtonClicked);
        restartGameButton?.onClick.AddListener(OnRestartGameButtonClicked);


        if (saveSlotSelector != null)
        {
            // SaveSlotSelector가 SaveManager의 이벤트를 구독하여 UI를 업데이트하고,
            // 플레이/삭제 버튼이 SaveManager의 LoadGame/DeleteSaveFile을 호출하도록 설정해야 합니다.
            // UIManager는 SaveSlotSelector의 RefreshUI만 호출하도록 합니다.
        }

    }

    void OnEnable()
    {
        // GameEvent 구독
        GameEvent.OnGameStart += OnGameStarted;               // 게임 시작 시
        GameEvent.OnGameOver += ShowGameOverPanel;            // 게임 오버 시
        GameEvent.OnStageStart += UpdateStageDisplay;         // 스테이지 시작 시 (스테이지 번호 UI 업데이트)
        GameEvent.OnPlayerHealthChanged += UpdatePlayerHealthUI; // 플레이어 체력 변경 시 (체력 바 UI 업데이트)
        //SaveManager.Instance.OnSaveFilesChanged += OnSaveFilesChanged; // 세이브 파일 변경 시 (저장/로드 메뉴 UI 갱신)

        // (선택 사항) 인벤토리 열림/닫힘 이벤트 구독 (ESC 키 처리 로직 간소화)
        // inventoryMain.OnInventoryOpened += OnInventoryOpened;
        // inventoryMain.OnInventoryClosed += OnInventoryClosed;
    }

    void OnDisable()
    {
        // GameEvent 구독 해제
        GameEvent.OnGameStart -= OnGameStarted;
        GameEvent.OnGameOver -= ShowGameOverPanel;
        GameEvent.OnStageStart -= UpdateStageDisplay;
        GameEvent.OnPlayerHealthChanged -= UpdatePlayerHealthUI;
        //SaveManager.Instance.OnSaveFilesChanged -= OnSaveFilesChanged;

        // (선택 사항) 인벤토리 이벤트 구독 해제
        // inventoryMain.OnInventoryOpened -= OnInventoryOpened;
        // inventoryMain.OnInventoryClosed -= OnInventoryClosed;

        // 버튼 이벤트 리스너 해제 (씬 전환 시 중복 연결 방지)
        newGameButton?.onClick.RemoveListener(OnNewGameButtonClicked);
        loadGameButton?.onClick.RemoveListener(OpenSavePanel);
        saveGameInPausePanelButton?.onClick.RemoveListener(OnSaveGameButtonClicked);
        exitGameButton?.onClick.RemoveListener(OnExitGameButtonClicked);
        restartGameButton?.onClick.RemoveListener(OnRestartGameButtonClicked);
    }

    // --- 이벤트 핸들러 ---
    private void OnGameStarted()
    {
        Debug.Log("UIManager: OnGameStarted 이벤트 수신. 게임 패널 활성화.");
        StartGamePanel();
        // 튜토리얼 대화 시작 (GameManager에서 호출하는 것이 더 적절할 수도 있습니다)
        // dialogueManager?.Init();
        // dialogueManager?.StartTutorial();
        PauseController.SetPause(false); // 게임 시작 시 일시정지 해제
    }

    public void ShowGameOverPanel()
    {
        Debug.Log("UIManager: OnGameOver 이벤트 수신. 게임 오버 패널 활성화.");
        gameOverPanel?.SetActive(true);
        // Time.timeScale = 0f; // GameManager에서 처리
        PauseController.SetPause(true); // UI 조작 가능하도록 시간은 멈추고 마우스는 활성화
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UpdateStageDisplay(int stageNumber)
    {
        if (stageNumberText != null)
        {
            stageNumberText.text = $"Stage: {stageNumber}";
            Debug.Log($"UIManager: 스테이지 UI 업데이트 - Stage {stageNumber}");
        }
    }

    public void UpdatePlayerHealthUI(float currentHealth, float maxHealth)
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }


    }

    private void OnSaveFilesChanged()
    {
        Debug.Log("UIManager: OnSaveFilesChanged 이벤트 수신. SaveSlotSelector UI 갱신.");
        saveSlotSelector?.RefreshUI();
    }

    // --- UI 패널 관리 메서드 ---
    public void CloseAllPanels()
    {
        menuPanel?.SetActive(false);
        gamePanel?.SetActive(false);
        pausePanel?.SetActive(false);
        savePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
    }

    public void OpenMenuPanel()
    {
        CloseAllPanels();
        menuPanel?.SetActive(true);
        PauseController.SetPause(true); // 메뉴에서는 게임 일시정지
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenSavePanel()
    {
        CloseAllPanels();
        savePanel?.SetActive(true);
        PauseController.SetPause(true); // 저장/불러오기 중에는 게임 일시정지
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // SavePanel 열릴 때마다 UI 갱신
        saveSlotSelector?.RefreshUI();
    }

    public void OpenPausePanel()
    {
        CloseAllPanels();
        pausePanel?.SetActive(true);
        PauseController.SetPause(true); // 일시정지 중에는 게임 일시정지
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenSettingsPanel()
    {
        CloseAllPanels();
        settingsPanel?.SetActive(true);
        PauseController.SetPause(true); // 설정 중에는 게임 일시정지
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGamePanel() // 인게임 UI 활성화 및 게임 재개
    {
        CloseAllPanels();
        gamePanel?.SetActive(true);
        PauseController.SetPause(false); // 게임 플레이 중에는 일시정지 해제
        Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
        Cursor.visible = false;                   // 커서 숨김
    }

    // --- UI 버튼 클릭 이벤트 핸들러 (GameManager에 작업 위임) ---
    public void OnNewGameButtonClicked()
    {
        Debug.Log("UIManager: 새 게임 버튼 클릭 -> GameManager에 요청");
        //GameManager.Instance.StartNewGame(); // GameManager에 새 게임 시작 요청
        // UIManager는 OnGameStarted 이벤트에 반응하여 UI를 업데이트합니다.
    }

    public void OnLoadGameButtonClicked(int slotIndex) // SaveSlotSelector에서 호출할 함수 (선택된 슬롯 전달)
    {
        Debug.Log($"UIManager: 로드 게임 버튼 클릭 (슬롯 {slotIndex}) -> GameManager에 요청");
        if (slotIndex > 0)
        {
            //GameManager.Instance.LoadGameFromSlot(slotIndex); // GameManager에 게임 로드 요청
        }
        else
        {
            Debug.LogWarning("UIManager: 유효하지 않은 슬롯이 선택되었습니다. 로드할 수 없습니다.");
        }
    }

    public void OnSaveGameButtonClicked() // 일시정지 메뉴 내 저장 버튼
    {
        Debug.Log("UIManager: 저장 버튼 클릭 -> SaveManager에 요청");
        //SaveManager.Instance.SaveCurrentGame(); // SaveManager에 현재 게임 저장 요청
        // SaveManager의 OnSaveFilesChanged 이벤트가 발생하여 SaveSlotSelector가 갱신될 것입니다.
    }

    public void OnDeleteGameButtonClicked(int slotIndex) // SaveSlotSelector에서 호출할 함수
    {
        Debug.Log($"UIManager: 삭제 버튼 클릭 (슬롯 {slotIndex}) -> SaveManager에 요청");
        if (slotIndex > 0)
        {
            //SaveManager.Instance.DeleteSaveFile(slotIndex); // SaveManager에 파일 삭제 요청
        }
        else
        {
            Debug.LogWarning("UIManager: 유효하지 않은 슬롯이 선택되었습니다. 삭제할 수 없습니다.");
        }
    }

    public void OnExitGameButtonClicked()
    {
        Debug.Log("UIManager: 게임 종료 버튼 클릭 -> GameManager에 요청");
        //GameManager.Instance.QuitGame(); // GameManager에 게임 종료 요청
    }

    public void OnRestartGameButtonClicked() // 게임 오버 패널의 재시작 버튼
    {
        Debug.Log("UIManager: 게임 오버 재시작 버튼 클릭 -> GameManager에 요청");
        //GameManager.Instance.RestartGame(); // GameManager에 게임 재시작 요청
    }

    // --- ESC 키 입력 처리 ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 인벤토리 활성 상태 처리 (가장 높은 우선순위)
            if (inventoryMain != null && inventoryMain.GetIsInventoryActive())
            {
                inventoryMain.CloseInventory();
            }
            // 게임 오버 패널이 활성화되어 있으면, ESC 키로 닫지 않고 재시작/메인 메뉴 등으로 유도합니다.
            else if (gameOverPanel != null && gameOverPanel.activeSelf)
            {
                // 아무것도 하지 않음 (사용자가 버튼을 클릭하도록 유도)
            }
            // 일시정지 패널 활성 상태 처리
            else if (pausePanel != null && pausePanel.activeSelf)
            {
                StartGamePanel(); // 게임 플레이 상태로 돌아감
            }
            // 게임 플레이 패널 활성 상태 처리
            else if (gamePanel != null && gamePanel.activeSelf)
            {
                OpenPausePanel(); // 일시정지 패널 엶
            }
            // 저장 패널 활성 상태 처리 (메뉴에서 접근했다면 메뉴로, 일시정지에서 접근했다면 일시정지로 돌아가게 로직을 설계할 수 있습니다.)
            // 여기서는 간단히 메인 메뉴로 돌아가도록 합니다.
            else if (savePanel != null && savePanel.activeSelf)
            {
                OpenMenuPanel();
            }
            // 설정 패널 활성 상태 처리 (일시정지에서 접근했다면 일시정지로 돌아감)
            else if (settingsPanel != null && settingsPanel.activeSelf)
            {
                OpenPausePanel();
            }
            // 그 외 (메인 메뉴 등)에서는 ESC 키 무시 또는 게임 종료
            else if (menuPanel != null && menuPanel.activeSelf)
            {
                // 게임 종료 팝업을 띄우거나, 아무것도 하지 않음
            }
        }
    }

}