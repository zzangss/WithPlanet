using UnityEngine;
using System.IO;

public class GameManagerSystem : MonoBehaviour
{
    // 싱글톤 패턴을 위한 정적 인스턴스
    public static GameManagerSystem Instance;

    // 게임 상태 변수
    public int stage;
    public float playTime;

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

    }

    // 새 게임을 시작하는 함수
    public void StartNewGame()
    {
        Debug.Log("새 게임 시작");

        // 기존 세이브 파일 삭제
        saveController.DeleteSaveFile();

        //초기화 작업 
        saveController.initializeWorld();
    }

    // 게임을 불러오는 함수
    public void LoadGame()
    {
        Debug.Log("저장된 게임 불러오기");

        // 저장 파일이 있는지 확인
        if (saveController.HasSaveFile())
        {
            // 저장된 데이터 로드
            saveController.LoadGame();


        }
        else
        {
            Debug.Log("저장된 파일이 없습니다. 새 게임을 시작합니다.");
            StartNewGame();
        }
    }

    //게임 저장함수
    public void SaveGame()
    {
        Debug.Log("게임 저장 중...");
        // 현재 게임 상태 저장
        saveController.SaveGame();
    }

    // 게임 종료 함수 
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        // 게임 종료 처리
        Application.Quit();

        // 에디터에서 실행 중인 경우
    }

   //저장파일 삭제
    public void DeleteSaveFile()
    {
        Debug.Log("저장 파일 삭제");
        saveController.DeleteSaveFile();
    }
}