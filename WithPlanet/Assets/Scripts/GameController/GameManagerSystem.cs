using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴을 위한 정적 인스턴스
    public static GameManager Instance;

    // 게임 상태 변수
    public int stage;
    public float playTime;

    // 관리할 시스템 스크립트 참조
    [SerializeField] private RandomSpawner randomSpawner;
    [SerializeField] private SaveController saveController;
    [SerializeField] private ItemDictionary itemDictionary;
    [SerializeField] private PlayerItemManager playerItemManager;

    // 세이브 파일 경로
    private string saveLocation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않게 설정
            saveLocation = Path.Combine(Application.persistentDataPath, "savefile.json");
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 경우 중복 생성 방지
        }
    }

    private void Start()
    {
        // Start 함수에서 각 스크립트 참조를 자동으로 찾습니다.
        // 하지만 인스펙터에서 직접 할당하는 것을 권장합니다.
        if (randomSpawner == null) randomSpawner = FindObjectOfType<RandomSpawner>();
        if (saveController == null) saveController = FindObjectOfType<SaveController>();
        if (itemDictionary == null) itemDictionary = FindObjectOfType<ItemDictionary>();
        if (playerItemManager == null) playerItemManager = FindObjectOfType<PlayerItemManager>();
    }

    // 새 게임을 시작하는 함수 (UI 버튼에 연결)
    public void StartNewGame()
    {
        Debug.Log("새 게임 시작");

        // 기존 세이브 파일이 있다면 삭제
        if (File.Exists(saveLocation))
        {
            saveController.DeleteSaveFile();
        }

        // 아이템 스폰
        if (randomSpawner != null)
        {
            randomSpawner.SpawnItems();
        }

        // 게임 시작 시 필요한 초기화 (예: 플레이어 위치 초기화 등)
    }

    // 저장된 게임을 불러오는 함수 (UI 버튼에 연결)
    public void LoadSavedGame()
    {
        Debug.Log("저장된 게임 불러오기");

        if (File.Exists(saveLocation))
        {
            saveController.LoadGame();
        }
        else
        {
            Debug.Log("저장된 파일이 없습니다. 새 게임을 시작합니다.");
            StartNewGame();
        }
    }
    //저장된 게임 제거
    public void DeleteSavedGame()
    {
        Debug.Log("저장된 게임 제거");
        if (File.Exists(saveLocation))
        {
            saveController.DeleteSaveFile();
           
        }
        else
        {
            Debug.Log("저장된 파일이 없습니다.");
        }
    }

    // 게임 저장 함수 (UI 버튼에 연결)
    public void SaveGame()
    {
        saveController.SaveGame();
    }


}
