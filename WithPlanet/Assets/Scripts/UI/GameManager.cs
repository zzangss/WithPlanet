using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public GameObject player;
    public int stage;
    public float playTime;

    public GameObject menuPanel;
    public GameObject GamePanel;
    void Start()
    {
        menuPanel.SetActive(true);
        GamePanel.SetActive(false);
    }
    public void GameStart()
    {
        Debug.Log("GameStart() ½ÇÇàµÊ");
        player.SetActive(true);
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        GamePanel.SetActive(true);

        
    }
}
