using UnityEngine;

public class UI_GameManager : MonoBehaviour
{
    public static UI_GameManager Instance;

    // UI �г� ����
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject savePanel;
    public DialogueManager dialogueManager;

    [SerializeField] private GameManagerSystem gameManagerSystem;
    [SerializeField] private SaveSlotSelector saveSlotSelector;
    [SerializeField] private InventoryMain inventoryMain;
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

        // ���� �Ŵ��� �ý��� �ʱ�ȭ
        gameManagerSystem = FindObjectOfType<GameManagerSystem>();
        //saveSlotSelector = FindObjectOfType<SaveSlotSelector>();
    }

    void Start()
    {
        // ���� ���� �� �޴� �г� ����
        OpenMenuPanel();
    }

    // UI �г��� ���� �ݴ� �Լ���
    public void OpenMenuPanel()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        savePanel.SetActive(false);
        PauseController.SetPause(true); // �޴��� ������ ���� �Ͻ�����
    }

    public void OpenSavePanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        savePanel.SetActive(true);

        // ���� ���¸� ����
        if (saveSlotSelector != null)
        {
            saveSlotSelector.RefreshUI();
        }
    }

    public void OpenPausePanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
        savePanel.SetActive(false);
        PauseController.SetPause(true); // �Ͻ����� �г��� ������ ���� �Ͻ�����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGamePanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        pausePanel.SetActive(false);
        savePanel.SetActive(false);
        PauseController.SetPause(false); // ���� �г��� ������ �Ͻ����� ����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

   
    //UI usage

    //�� ���� ����
    public void StartNewGame()
    {
        if (gameManagerSystem.StartNewGame())
        {
            if (saveSlotSelector != null)
            {
                saveSlotSelector.RefreshUI();
            }
           
            dialogueManager.StartTutorial();
            PauseController.SetPause(false);
            StartGamePanel();
        }
        else
        {
            Debug.Log("UI �Ŵ���: �� ���� ���� ����. ������ �������� �ʽ��ϴ�.");
        }

    }

    // paly button click
    public void StartSelectedGame()
    {
        if(saveSlotSelector == null)
        {
            Debug.Log("save slot selector가 null");
            return;
        }
        int selectedSlot = saveSlotSelector.GetSelectedSlot();

        if (selectedSlot > 0)
        {
            if (gameManagerSystem.HasSaveFile(selectedSlot))
            {

                if (saveSlotSelector != null)
                {
                    saveSlotSelector.RefreshUI();
                }
                gameManagerSystem.LoadGame(selectedSlot);
                StartGamePanel();
            }
            else
            {
                // 파일이 존재하지 않으면 경고 메시지를 띄웁니다.
                Debug.Log($"선택한 슬롯 {selectedSlot}은 비어있습니다!");
            }
        }
        else
        {
            Debug.Log("게임을 시작하려면 슬롯을 먼저 선택하세요!");
        }

    }

    // ���� ��ư�� ����

    // ���� ���� 
    public void SaveGame()
    {
        gameManagerSystem.SaveGame();
        
    }

    //��������
    public void ExitGame()
    {
        gameManagerSystem.QuitGame(); // ���� ����
    }

    // ���̺� ���� ���� (���� ��ȣ�� �����ϱ�)
    public void DeleteGame()
    {
        int selectedSlot = saveSlotSelector.GetSelectedSlot();
        {
            gameManagerSystem.DeleteSaveFile(selectedSlot);
            // ���� ���¸� ����
            if (saveSlotSelector != null)
            {
                saveSlotSelector.RefreshUI();
            }

            saveSlotSelector.SetSelectedSlot(-1);
        }
    }


    //�޴��� ���ư���
    public void ReturnToMenu()
    {
        OpenMenuPanel(); // �޴� �г� ����
    }



    public 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ���� ���� ��
            if (gamePanel.activeSelf && !inventoryMain.GetIsInventoryActive())
            {
                OpenPausePanel();
            }
            else if (inventoryMain.GetIsInventoryActive())
            {
                inventoryMain.CloseInventory();
            }
            // ���� �� ������ ��
            else if (pausePanel.activeSelf)
            {
                StartGamePanel();
            }
            // �޴��� �� ���̺� -> �޴��� ���ư���
            else if (savePanel.activeSelf)
            {
                OpenMenuPanel();
            }
           
        }
    }
}