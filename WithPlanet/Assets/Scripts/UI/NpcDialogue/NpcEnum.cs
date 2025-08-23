using System;
using UnityEngine;

using UnityEditor;

public enum Type
{
    Object = 0,
    Miluna = 100,
    BoxMonster = 200,
    Boss = 300
}

public enum State
{
    Opening = 0,       // 게임 시작 직후 Boss가 설명하는 오프닝
    PreMinigame = 1,   // 미니게임 시작 전
    PostMinigame = 2   // 미니게임 종료 후
}

public enum Face
{
    Default,
    Angry,
    Smile
}
