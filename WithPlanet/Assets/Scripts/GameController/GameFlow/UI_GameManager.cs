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

    //새게임시작
    public void StartNewGame()
    {
        gameManagerSystem.StartNewGame(); // 새 게임 시작
        PauseController.SetPause(true);
        StartGamePanel();
        dialogueManager.StartTutorial();
    }

    //게임시작
    public void StartGame()
    {
        gameManagerSystem.LoadGame(); // 게임 시작 시 저장된 게임 불러오기
        StartGamePanel();
    }

    //게임저장
    public void SaveGame()
    {
        gameManagerSystem.SaveGame(); // 게임 저장
    }

    //게임종료
    public void ExitGame()
    {
        gameManagerSystem.QuitGame(); // 게임 종료
    }

    //세이브파일 삭제
    public void DeleteSaveFile()
    {
      gameManagerSystem.DeleteSaveFile(); // 세이브 파일 삭제
    }

    //메뉴로 돌아가기
    public void ReturnToMenu()
    {
        OpenMenuPanel(); // 메뉴 패널 열기
    }

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