using UnityEngine;
using System.IO;

public class GameManagerSystem : MonoBehaviour
{
    // �̱��� ������ ���� ���� �ν��Ͻ�
    public static GameManagerSystem Instance;

    // ���� ���� ����
    public int stage;
    public float playTime;

    // ���̺����� ����
    public bool hasSaveFile;

    //���̺� ���� ��ȣ ����
    private int currentSaveSlot = 0;

    // ������ �ý��� ��ũ��Ʈ ���� 
    [SerializeField] private SaveController saveController;
    [SerializeField] private SaveSlotSelector saveSlotSelector;

    private void Awake()
    {
        // �̱��� �������� GameManagerSystem �ν��Ͻ��� �ϳ��� �����ϵ��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� �ʰ� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �����ϴ� ��� �ߺ� ���� ����
        }
    }

    private void Start()
    {
       
        if (!saveController.HasSaveFile(0))
        {
            InitialSave();
        }
        UpdateHasSaveFileStatus();
    }

    void Update()
    {
        // ���� ���� ������Ʈ (��: �÷��� �ð� ����)
        playTime += Time.deltaTime;
        //  stage�� 1�� ������Ű�� ���� (���߿� �߰�)
    }

    // �� ������ �����ϴ� �Լ�
    public bool StartNewGame()
    {
        // 1. �� ���� ã��
        int availableSlot = -1;
        for (int i = 1; i <= 3; i++)
        {
            if (!saveController.HasSaveFile(i))
            {
                availableSlot = i;
                break;
            }
        }

        // 2. �� ������ �ִ��� Ȯ��
        if (availableSlot != -1)
        {
            // 3. ���� ��: ������ ������ ���� ��ȣ ����
            currentSaveSlot = availableSlot;

            // 4. ���� ���� �ʱ�ȭ
            saveController.LoadGame(0);
            saveController.initializeWorld();
            playTime = 0f;

            return true; // ����!
        }
        else
        {
            // 5. ���� ��:
            Debug.Log("�� ���� ���� ����: ��� ������ �� á���ϴ�.");
            return false; // ����!
        }
    }

    //file exist conform
   public bool HasSaveFile(int selectedSlot)
    {
        return saveController.HasSaveFile(selectedSlot);
    }


    // ������ �ҷ����� �Լ� (���� ��ȣ ����)
    public void LoadGame(int slotIndex)
    {
        Debug.Log($"���� {slotIndex}���� ����� ���� �ҷ�����");
        saveController.LoadGame(slotIndex);

        // ���� �÷��� ���� ���� ��ȣ�� �����մϴ�.
        currentSaveSlot = slotIndex;
        UpdateHasSaveFileStatus();
    }


    // ���� ���� �Լ� (���� ��ȣ ����)
    public void SaveGame(int slotIndex)
    {
        
        Debug.Log($"���� {slotIndex}�� ���� ����");
        saveController.SaveGame(slotIndex,playTime);
    }

    // �������� ���� ���� �Լ� 
    public void SaveGame()
    {
        Debug.Log($"���� {currentSaveSlot}�� ���� ����");
        if(currentSaveSlot != 0)
        {
            saveController.SaveGame(currentSaveSlot, playTime);
        }
        
    }

    // �ʱ� ���¸� �����ϴ� �Լ� (���� �ʹݿ� �� ���� ���, �� ���� ���� �� ����� ���̺� ����)
    public void InitialSave()
    {
        Debug.Log("�ʱ� ���� ���� ����");
        saveController.SaveGame(0,0f); // 0�� ���Կ� ����
    }

    // ���� ���� �Լ�
    public void QuitGame()
    {
        Debug.Log("game end");
        Application.Quit();
    }

    // ���̺� ���� ���� (���� ��ȣ ����)
    public void DeleteSaveFile(int slotIndex)
    {
        Debug.Log($"���� {slotIndex}�� ���̺� ���� ����");
        saveController.DeleteSaveFile(slotIndex);
        saveSlotSelector.SetSelectedSlot(-1);
        UpdateHasSaveFileStatus();
    }

    // ���� ���̺� ���Ե��� ���� ���θ� Ȯ���ϰ� hasSaveFile ������ ������Ʈ�մϴ�.
    private void UpdateHasSaveFileStatus()
    {
        hasSaveFile = false;
        for (int i = 1; i <= 3; i++)
        {
            if (saveController.HasSaveFile(i))
            {
                hasSaveFile = true;
                break;
            }
        }
    }
}