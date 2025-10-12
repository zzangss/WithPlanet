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
    PreMinigame1 = 1,  // 미니게임 시작 전
    PreMinigame1_X = 2,
    PreMinigame1_O = 3,   
    PostMinigame1Success = 4,  // 미니게임 종료 후
    PostMinigame1Fail = 5,
}

public enum Face
{
    Default,
    Angry,
    Smile
}
