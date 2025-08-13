using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool isPaused { get; private set; } = false; // 일시정지 상태를 나타냄

    public static void SetPause(bool pause)
    {
        isPaused = pause; // 일시정지 상태
        Time.timeScale = pause ? 0f : 1f; // 일시정지 상태에 따라 시간 스케일을 조정
    }
}
