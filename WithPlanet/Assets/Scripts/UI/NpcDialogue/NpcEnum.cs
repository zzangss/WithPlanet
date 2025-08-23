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
    Default = 0,    // 오브젝트
    Talk = 1,   // 처음 만났을 때 하는 대사
    Hint = 2,   // ###를 nextQuestNPC로 변경
    Quest = 3,  // Quest대사는 끝나고 퀘스트가 주어짐
}

public enum Face
{
    Default,
    Angry,
    Smile
}
