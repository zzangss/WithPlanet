using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/dialogue data")]
public class TalkData : ScriptableObject
{
    public Type type;
    public talkContent[] talkContent;
}

[Serializable]
public class talkContent
{
    public State state;
    public Content[] content;
}

[Serializable]
public class Content
{
    public Face face;
    [TextArea(2, 6)]
    public string script;
}