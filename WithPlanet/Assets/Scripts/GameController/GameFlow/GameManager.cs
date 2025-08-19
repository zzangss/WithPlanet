using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    
    public GameObject gameCam;
    public GameObject player;
    public int stage;
    public float playTime;

    public GameObject menuPanel;
    public GameObject GamePanel;
    public GameObject PausePanel;
    public GameObject savePanel;


    void Start()
    {
        MenuOpen();
     }

    public void GameStart()
    {
        PauseController.SetPause(false); // 일시정지 해제

        player.SetActive(true);
        gameCam.SetActive(true);
        savePanel.SetActive(false);
        menuPanel.SetActive(false);
        GamePanel.SetActive(true);
    }
    //메뉴열기
    public void MenuOpen()
    {
        PauseController.SetPause(true); // 일시정지 상태로 전환
        menuPanel.SetActive(true);
        GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        savePanel.SetActive(false);
    }

    //세이브파일 열기
    public void SaveFileOpen()
    {
        menuPanel.SetActive(false);
        savePanel.SetActive(true);
    }

    //세이브파일 닫기
    public void SaveFileClose()
    {
        savePanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //게임중일 때
            if (GamePanel.activeSelf)
            {
                GamePanel.SetActive(false);
                PausePanel.SetActive(true);
                PauseController.SetPause(true); // 일시정지 상태로 전환
            }
            //게임중 멈춤일 때
            else if (PausePanel.activeSelf)
            {
                PausePanel.SetActive(false);
                GamePanel.SetActive(true);
                PauseController.SetPause(false); // 일시정지 해제
            }
            //메뉴일 때 세이브 -> 메뉴
            else if( !menuPanel.activeSelf && savePanel.activeSelf)
            {
                SaveFileClose();
            }
            
        }
    }
}
