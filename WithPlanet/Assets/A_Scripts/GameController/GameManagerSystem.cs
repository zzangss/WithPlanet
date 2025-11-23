using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement; // [필수] 씬 관리를 위해 추가

public class GameManagerSystem : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManagerSystem Instance;

    [Header("Settings")]
    public string startingSceneName = "m_movestage"; // [필수] 시작할 씬 이름 (인스펙터에서 수정 가능)

    // 게임 상태 변수
    public int stage;
    public float playTime;

    // 세이브 파일 존재 여부
    public bool hasSaveFile;

    // 현재 플레이 중인 슬롯 번호
    private int currentSaveSlot = 0;

    // 외부 시스템 참조
    [SerializeField] private SaveController saveController;
    [SerializeField] private SaveSlotSelector saveSlotSelector;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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
        playTime += Time.deltaTime;
    }

    // =========================================================================
    // [수정됨] 새 게임 시작 로직 (씬 이동 포함)
    // =========================================================================
    public bool StartNewGame()
    {
        // 1. 빈 슬롯 탐색
        int availableSlot = -1;
        for (int i = 1; i <= 3; i++)
        {
            if (!saveController.HasSaveFile(i))
            {
                availableSlot = i;
                break;
            }
        }

        // 2. 빈 슬롯이 있다면 시작 절차 진행
        if (availableSlot != -1)
        {
            Debug.Log($"새 게임 시작: {availableSlot}번 슬롯 확보. '{startingSceneName}' 씬으로 이동합니다.");

            // 코루틴을 실행하여 '씬 이동 -> 대기 -> 초기화'를 순차적으로 처리
            StartCoroutine(LoadNewGameRoutine(availableSlot));

            return true; // UIManager에게 "성공했다"고 알림
        }
        else
        {
            Debug.Log("새 게임 불가: 모든 슬롯 사용 중");
            return false;
        }
    }

    // [추가됨] 새 게임 씬 로딩 코루틴
    private System.Collections.IEnumerator LoadNewGameRoutine(int slotIndex)
    {
        // 1. 현재 슬롯 설정 및 시간 초기화
        currentSaveSlot = slotIndex;
        playTime = 0f;

        // 2. 시작 씬 로드 (비동기)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(startingSceneName);

        // 로딩이 끝날 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 3. 씬 로딩 직후 한 프레임 더 대기 (안정성 확보)
        yield return null;

        // 4. 월드/데이터 초기화 (SaveController가 새 씬의 참조를 찾도록 유도)
        // (참고: SaveController의 FindReferences가 씬 로드 시 자동 호출되지 않는다면 여기서 호출 필요할 수 있음)
        saveController.initializeWorld();

        // 5. 플레이어를 StartPoint로 이동
        MovePlayerToStartPoint();

        // 6. 바로 저장 한번 하기 (선택 사항: 시작하자마자 데이터 생성)
        SaveGame(currentSaveSlot);
    }

    // 플레이어를 StartPoint로 강제 이동시키는 함수
    private void MovePlayerToStartPoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject startPoint = GameObject.Find("StartPoint");

        if (player != null && startPoint != null)
        {
            Debug.Log($"플레이어({player.name})를 StartPoint 위치로 이동.");

            // 물리 간섭 방지
            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = startPoint.transform.position;
            player.transform.rotation = startPoint.transform.rotation;

            if (cc != null) cc.enabled = true;
        }
        else
        {
            Debug.LogWarning("[MovePlayerToStartPoint] Player 또는 StartPoint를 찾을 수 없습니다.");
        }
    }

    // =========================================================================

    public bool HasSaveFile(int selectedSlot)
    {
        return saveController.HasSaveFile(selectedSlot);
    }

    public void LoadGame(int slotIndex)
    {
        Debug.Log($"슬롯 {slotIndex}번 로드");
        saveController.LoadGame(slotIndex); // SaveController 안에서 씬 이동 처리함
        currentSaveSlot = slotIndex;
        UpdateHasSaveFileStatus();
    }

    public void SaveGame(int slotIndex)
    {
        Debug.Log($"슬롯 {slotIndex}번 저장");
        saveController.SaveGame(slotIndex, playTime);
    }

    public void SaveGame()
    {
        Debug.Log($"현재 슬롯({currentSaveSlot}번) 저장");
        if (currentSaveSlot != 0)
        {
            saveController.SaveGame(currentSaveSlot, playTime);
        }
    }

    public void InitialSave()
    {
        Debug.Log("초기 데이터(0번) 생성");
        saveController.SaveGame(0, 0f);
    }

    public void RestartGame()
    {
        Debug.Log("게임 재시작 실행");
        if (currentSaveSlot != 0)
        {
            LoadGame(currentSaveSlot);
        }
        else
        {
            StartNewGame();
        }
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    public void DeleteSaveFile(int slotIndex)
    {
        Debug.Log($"슬롯 {slotIndex}번 삭제");
        saveController.DeleteSaveFile(slotIndex);
        saveSlotSelector.SetSelectedSlot(-1);
        UpdateHasSaveFileStatus();
    }

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