using UnityEngine;

public class UI_GameManager : MonoBehaviour
{
    public static UI_GameManager Instance;

    // UI 패널 참조
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject savePanel;
    public GameObject inventoryPanel;
    public DialogueManager dialogueManager;

    [SerializeField] private GameManagerSystem gameManagerSystem;
    [SerializeField] private SaveSlotSelector saveSlotSelector;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 게임 매니저 시스템 초기화
        gameManagerSystem = FindObjectOfType<GameManagerSystem>();

        // 세이브 슬롯 선택기 초기화
        saveSlotSelector = FindObjectOfType<SaveSlotSelector>();

    }

    void Start()
    {
        // 게임 시작 시 메뉴 패널 열기
        OpenMenuPanel();
    }

    // UI 패널을 열고 닫는 함수들
    public void OpenMenuPanel()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        savePanel.SetActive(false);
        PauseController.SetPause(true); // 메뉴가 열리면 게임 일시정지
    }



    public void OpenSavePanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        savePanel.SetActive(true);
    }

    public void OpenPausePanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
        savePanel.SetActive(false);
        PauseController.SetPause(true); // 일시정지 패널이 열리면 게임 일시정지
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGamePanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        pausePanel.SetActive(false);
        savePanel.SetActive(false);
        PauseController.SetPause(false); // 게임 패널이 열리면 일시정지 해제
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenInventoryPanel()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf); // 인벤토리 패널 토글
        if (inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
        }
        else
        {
            inventoryPanel.SetActive(true);
        }
    }
    //UI 버튼 연결 

    //새 게임 시작
    public void StartNewGame()
    {
        gameManagerSystem.NewGameLoad(); // 새 게임 시작
        StartGamePanel(); // 게임 패널 열기
    }

    //게임 시작 (사용자가 선택한 슬롯에 따라)
    public void StartSelectedGame()
    {
        int selectedSlot = saveSlotSelector.GetSelectedSlot();

        if (selectedSlot != -1)
        {
            // 선택된 슬롯이 있을 경우에만 게임 불러오기
            gameManagerSystem.LoadGame(selectedSlot);
            StartGamePanel();
        }
        else
        {
            // 슬롯이 선택되지 않았을 경우 경고 메시지 표시
            Debug.Log("게임을 시작하려면 먼저 슬롯을 선택하세요!");
        }
    }

    // 슬롯 버튼에 연결

    // 게임 저장 
    public void SaveGame()
    {
        gameManagerSystem.SaveGame();
        // 저장 후 UI 갱신 로직 필요 (예: 파일 생성 날짜 표시)
    }

    //게임종료
    public void ExitGame()
    {
        gameManagerSystem.QuitGame(); // 게임 종료
    }

    // 세이브 파일 삭제 (슬롯 번호로 삭제하기)
    public void OnDeleteButtonClicked(int slotIndex)
    {
        gameManagerSystem.DeleteSaveFile(slotIndex);
        // 삭제 후 UI 갱신 로직 필요
    }


    //메뉴로 돌아가기
    public void ReturnToMenu()
    {
        OpenMenuPanel(); // 메뉴 패널 열기
    }



    public 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 게임 중일 때
            if (gamePanel.activeSelf)
            {
                OpenPausePanel();
            }
            // 게임 중 멈춤일 때
            else if (pausePanel.activeSelf)
            {
                StartGamePanel();
            }
            // 메뉴일 때 세이브 -> 메뉴로 돌아가기
            else if (savePanel.activeSelf)
            {
                OpenMenuPanel();
            }
        }

       
    }
}