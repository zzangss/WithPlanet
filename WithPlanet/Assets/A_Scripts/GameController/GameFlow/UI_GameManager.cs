using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// UI의 '기능'을 정의하는 스크립트입니다.
/// 버튼 연결은 코드가 아닌, 유니티 에디터(Inspector)의 OnClick() 이벤트에서 직접 연결합니다.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panel References")]
    // 패널들은 껐다 켰다 제어해야 하므로 변수로 가지고 있습니다.
    public GameObject menuPanel;
    public GameObject gamePanel;    // 인게임 HUD (체력, 스테이지 등)
    public GameObject pausePanel;   // 일시정지 화면
    public GameObject savePanel;    // 저장/로드 화면
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("External Managers")]
    [SerializeField] private SaveSlotSelector saveSlotSelector;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("In-Game HUD")]
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Text stageNumberText;

    private void Awake()
    {
        // 싱글톤 패턴: 게임 내에 단 하나만 존재하며, 씬이 바뀌어도 파괴되지 않음
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 게임 시작 시 메인 메뉴를 엽니다.
        OpenMenuPanel();

        // SaveSlotSelector 초기화 (있다면)
        if (saveSlotSelector != null)
        {
            // 필요 시 초기화 로직 추가
        }
    }

    void OnEnable()
    {
        // 게임 이벤트 구독 (옵저버 패턴)
        GameEvent.OnGameStart += OnGameStarted;
        GameEvent.OnGameOver += ShowGameOverPanel;
        GameEvent.OnStageStart += UpdateStageDisplay;
        GameEvent.OnPlayerHealthChanged += UpdatePlayerHealthUI;
    }

    void OnDisable()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        GameEvent.OnGameStart -= OnGameStarted;
        GameEvent.OnGameOver -= ShowGameOverPanel;
        GameEvent.OnStageStart -= UpdateStageDisplay;
        GameEvent.OnPlayerHealthChanged -= UpdatePlayerHealthUI;
    }

    // ==================================================================================
    // [버튼 기능 함수들] 
    // Inspector의 OnClick()에서 이 함수들을 직접 드래그해서 연결하세요.
    // ==================================================================================

    // 1. 새 게임 버튼 기능
    // "New Game" 버튼에 연결할 함수
    public void OnNewGameClicked()
    {
        // 1. 혹시 슬롯이 선택되어 있었다면 해제. (선택된 슬롯 로드 방지)
        if (saveSlotSelector != null)
        {
            saveSlotSelector.SetSelectedSlot(-1);
        }

        Debug.Log("UIManager: 새 게임 자동 시작 시도 (빈 슬롯 탐색)");

        // 2. GameManager에게 "빈 자리 찾아서 새 게임 시작해줘!"라고 요청
        // (성공하면 true, 자리 없으면 false가 반환됨)
        bool isStarted = GameManagerSystem.Instance.StartNewGame();

        if (isStarted)
        {
            // 성공! 게임 화면으로 넘어감
            StartGamePanel();
        }
        else
        {
            // 실패! (빈 슬롯이 없음)
            Debug.LogWarning("알림: 빈 슬롯이 없습니다! 기존 데이터를 삭제해야 합니다.");

            // ★ 여기에 "슬롯이 꽉 찼습니다!" 팝업창을 띄우는 코드를 넣으면 됩니다.
            // 예시: warningPopup.SetActive(true);
        }
    }

    // 2. 게임 불러오기 버튼 기능 (저장 슬롯 패널 열기)
    public void OnOpenLoadPanelClicked()
    {
        OpenSavePanel();
    }

    // 3. (슬롯 선택 후) 실제 로드 실행 기능
    // Inspector에서 각 슬롯 버튼에 연결할 때, 매개변수(Int)에 1, 2, 3을 적어주세요.
    public void OnLoadSlotClicked(int slotIndex)
    {
        Debug.Log($"UIManager: 슬롯 {slotIndex}번 로드 요청");
        if (slotIndex > 0)
        {
            GameManagerSystem.Instance.LoadGame(slotIndex);
            StartGamePanel(); // 로드 후 게임 화면 활성화
        }
    }

    // 4. 저장 버튼 기능 (일시정지 화면 등에서 사용)
    public void OnSaveGameClicked()
    {
        Debug.Log("UIManager: 현재 상태 저장 요청");
        GameManagerSystem.Instance.SaveGame();

        // 저장이 완료되면 UI 갱신 (선택 사항)
        if (saveSlotSelector != null) saveSlotSelector.RefreshUI();
    }

    // 5. 삭제 버튼 기능
    public void OnDeleteSlotClicked(int slotIndex)
    {
        Debug.Log($"UIManager: 슬롯 {slotIndex}번 삭제 요청");
        if (slotIndex > 0)
        {
            GameManagerSystem.Instance.DeleteSaveFile(slotIndex);
        }
    }

    // 6. 게임 종료 버튼 기능
    public void OnExitGameClicked()
    {
        Debug.Log("UIManager: 게임 종료 요청");
        GameManagerSystem.Instance.QuitGame();
    }

    // 7. 재시작 버튼 기능 (게임 오버 시)
    public void OnRestartGameClicked()
    {
        Debug.Log("UIManager: 재시작 요청");
        GameManagerSystem.Instance.RestartGame();
    }

    // 8. 게임 슬롯 선택 -> start 버튼 눌러 특정 세이브 파일 start 가능 
    public void OnStartSelectedGameClicked()
    {
        // [핵심 변경] UIManager가 직접 기억하지 말고, SaveSlotSelector에게 물어봅니다!
        // "지금 선택된 슬롯 번호가 몇 번이야?"
        int selectedSlot = -1;
        if (saveSlotSelector != null)
        {
            selectedSlot = saveSlotSelector.GetSelectedSlot();
        }

        // 아무것도 선택 안 함 (-1)
        if (selectedSlot == -1)
        {
            Debug.LogWarning("UIManager: 게임을 시작하려면 슬롯을 먼저 선택해주세요!");
            return;
        }

        Debug.Log($"UIManager: 선택된 {selectedSlot}번 슬롯으로 게임 시작!");

        // 선택된 슬롯에 세이브 파일이 있으면 -> 로드
        if (GameManagerSystem.Instance.HasSaveFile(selectedSlot))
        {
            GameManagerSystem.Instance.LoadGame(selectedSlot);
            StartGamePanel();
        }
        else
        {
            // 파일 없으면 -> 새 게임으로 초기화 후 시작
            GameManagerSystem.Instance.SaveGame(selectedSlot); // 0초로 초기화
            GameManagerSystem.Instance.LoadGame(selectedSlot); // 로드
            StartGamePanel();
        }
    }

    // 9. 특정 save 파일 눌러서 삭제 가능 
    public void OnDeleteSelectedSlotClicked()
    {
        // 1. SaveSlotSelector에게 지금 선택된 슬롯 번호를 물어봅니다.
        int selectedSlot = -1;
        if (saveSlotSelector != null)
        {
            selectedSlot = saveSlotSelector.GetSelectedSlot();
        }

        // 2. 아무것도 선택하지 않았다면 무시합니다.
        if (selectedSlot == -1)
        {
            Debug.LogWarning("UIManager: 삭제할 슬롯을 먼저 선택해주세요!");
            return;
        }

        // 3. (안전장치) 빈 슬롯을 삭제하려고 하면 무시합니다.
        // (GameManagerSystem에 HasSaveFile 함수가 있으니 활용합니다)
        if (GameManagerSystem.Instance.HasSaveFile(selectedSlot) == false)
        {
            Debug.LogWarning("UIManager: 빈 슬롯이라 삭제할 데이터가 없습니다.");
            return;
        }

        // 4. 매니저에게 "이 번호 삭제해줘!"라고 요청합니다.
        Debug.Log($"UIManager: {selectedSlot}번 슬롯 데이터 삭제 요청");
        GameManagerSystem.Instance.DeleteSaveFile(selectedSlot);

        // 5. 삭제가 완료되면 SaveSlotSelector의 화면(UI)도 새로고침 해줍니다.
        // (이미 GameManagerSystem.DeleteSaveFile 안에서 갱신 로직이 있다면 생략 가능하지만, 안전하게 한번 더 호출)
        saveSlotSelector.RefreshUI();
    }

    // ==================================================================================
    // [이벤트 리스너 & 패널 제어]
    // ==================================================================================

    private void OnGameStarted()
    {
        StartGamePanel();
        PauseController.SetPause(false);
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel?.SetActive(true);
        PauseController.SetPause(true); // 시간 정지
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 보이기
        Cursor.visible = true;
    }

    public void UpdateStageDisplay(int stageNumber)
    {
        if (stageNumberText != null)
        {
            stageNumberText.text = $"Stage: {stageNumber}";
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

    // 모든 패널 끄기 (초기화 용도)
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
        PauseController.SetPause(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenSavePanel()
    {
        CloseAllPanels();
        savePanel?.SetActive(true);
        PauseController.SetPause(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        saveSlotSelector?.RefreshUI(); // 패널 열 때 슬롯 상태 갱신
    }

    public void OpenPausePanel()
    {
        CloseAllPanels();
        pausePanel?.SetActive(true);
        PauseController.SetPause(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenSettingsPanel()
    {
        CloseAllPanels();
        settingsPanel?.SetActive(true);
        PauseController.SetPause(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGamePanel() // 인게임 플레이 상태로 전환
    {
        CloseAllPanels();
        gamePanel?.SetActive(true);
        PauseController.SetPause(false); // 시간 흐름 재개
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 잠금 (FPS/TPS 게임 등)
        Cursor.visible = false;
    }


    // ==================================================================================
    // [ESC 키 입력 처리]
    // ==================================================================================
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 1. 게임 오버 상태일 때는 ESC 무시 (또는 메뉴로 나가기 등 처리 가능)
            if (gameOverPanel != null && gameOverPanel.activeSelf)
            {
                return;
            }

            // 2. 일시정지 상태 -> 게임으로 복귀
            if (pausePanel != null && pausePanel.activeSelf)
            {
                StartGamePanel();
            }
            // 3. 게임 플레이 중 -> 일시정지 화면 열기
            else if (gamePanel != null && gamePanel.activeSelf)
            {
                OpenPausePanel();
            }
            // 4. 저장/설정 패널 -> 이전 화면(메뉴 또는 일시정지)으로 복귀
            // (여기서는 편의상 메인 메뉴로 보내거나 일시정지로 보냅니다)
            else if ((savePanel != null && savePanel.activeSelf) || (settingsPanel != null && settingsPanel.activeSelf))
            {
                // 게임 중이었으면 일시정지로, 아니면 메뉴로 가야하는데, 
                // 일단 간단하게 이전 패널 로직 대신 OpenPausePanel로 통일하거나 메뉴로 보냄
                OpenMenuPanel();
            }
        }
    }
}