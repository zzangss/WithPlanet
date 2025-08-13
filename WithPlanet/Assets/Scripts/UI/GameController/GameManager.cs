using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    public GameObject gameCam;
    public GameObject player;
    public int stage;
    public float playTime;

    public GameObject menuPanel;
    public GameObject GamePanel;
    public GameObject PausePanel;

    void Start()
    {
        menuPanel.SetActive(true);
        GamePanel.SetActive(false);
        PausePanel.SetActive(false);
    }
    public void GameStart()
    {
        player.SetActive(true);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        GamePanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePanel.activeSelf)
            {
                GamePanel.SetActive(false);
                PausePanel.SetActive(true);
                PauseController.SetPause(true); // 일시정지 상태로 전환
            }
            else if (PausePanel.activeSelf)
            {
                PausePanel.SetActive(false);
                GamePanel.SetActive(true);
                PauseController.SetPause(false); // 일시정지 해제
            }
        }
    }
}
