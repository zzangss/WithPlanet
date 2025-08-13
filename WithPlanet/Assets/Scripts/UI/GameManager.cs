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
    public GameObject SettingsPanel;

    void Start()
    {
        menuPanel.SetActive(true);
        GamePanel.SetActive(false);
        SettingsPanel.SetActive(false);
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
                SettingsPanel.SetActive(true);
            }
            else if (SettingsPanel.activeSelf)
            {
                SettingsPanel.SetActive(false);
                GamePanel.SetActive(true);
            }
        }
    }
}
