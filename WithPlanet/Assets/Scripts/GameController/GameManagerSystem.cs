using UnityEngine;
using System.IO;

public class GameManagerSystem : MonoBehaviour
{
    // 싱글톤 패턴을 위한 정적 인스턴스
    public static GameManagerSystem Instance;

    // 게임 상태 변수
    public int stage;
    public float playTime;

    // 세이브파일 존재
    public bool hasSaveFile;

    //세이브 파일 번호 저장
    private int currentSaveSlot = 0;

    // 관리할 시스템 스크립트 참조 
    [SerializeField] private SaveController saveController;
    


    private void Awake()
    {
        // 싱글톤 패턴으로 GameManagerSystem 인스턴스가 하나만 존재하도록 보장
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않게 설정
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 경우 중복 생성 방지
        }
    }

  

    private void Start()
    {
        saveController = FindObjectOfType<SaveController>();
        // 게임 시작 시 초기 상태 파일이 없으면 생성
        if (!saveController.HasSaveFile(0))
        {
            InitialSave();
        }
    }

    void Update()
    {
        // 게임 상태 업데이트 (예: 플레이 시간 증가)
        playTime += Time.deltaTime;
        //  stage를 1씩 증가시키는 로직 (나중에 추가)

    }

    // 새 게임을 시작하는 함수
    public void NewGameLoad()
    {
        Debug.Log("새 게임 시작");

        saveController.LoadGame(0); // 0번 슬롯 (초기 상태) 불러오기
        saveController.initializeWorld(); // 월드 아이템만 초기화

        currentSaveSlot = 1; // 현재 슬롯을 첫번째 슬롯으로 설정
    }

    // 게임을 불러오는 함수 (슬롯 번호 지정)
    public void LoadGame(int slotIndex)
    {
        Debug.Log($"슬롯 {slotIndex}에서 저장된 게임 불러오기");
        saveController.LoadGame(slotIndex);

        // 현재 플레이 중인 슬롯 번호를 저장합니다.
        currentSaveSlot = slotIndex;
    }

    // 게임 저장 함수 (슬롯 번호 지정)
    public void SaveGame()
    {
        Debug.Log($"슬롯 {currentSaveSlot}에 게임 저장");
        if(currentSaveSlot != 0)
        {
            saveController.SaveGame(currentSaveSlot);
        }
        
    }

    // 초기 상태를 저장하는 함수 (게임 초반에 한 번만 사용, 새 게임 시작 시 사용할 세이브 파일)
    public void InitialSave()
    {
        Debug.Log("초기 상태 파일 생성");
        saveController.SaveGame(0); // 0번 슬롯에 저장
    }

    // 게임 종료 함수
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    // 세이브 파일 삭제 (슬롯 번호 지정)
    public void DeleteSaveFile(int slotIndex)
    {
        Debug.Log($"슬롯 {slotIndex}의 세이브 파일 삭제");
        saveController.DeleteSaveFile(slotIndex);
    }
}