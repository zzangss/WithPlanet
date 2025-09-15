using System;
using UnityEngine;


[Serializable]
public enum LineKind { Text, Choice }

[Serializable]
public class DialogueEntry
{
    public LineKind kind = LineKind.Text;

    // Text
    [TextArea(2, 6)] public string text;

    // Choice
    [TextArea(1, 3)] public string choicePrompt;
    public string optionO = "O";
    public string optionX = "X";
    public int nextIndexIfO = -1;   // -1이면 기본 +1
    public int nextIndexIfX = -1;

    public static DialogueEntry Line(string t) => new DialogueEntry { kind = LineKind.Text, text = t };
    public static DialogueEntry Choice(string prompt, string o = "O", string x = "X", int oNext = -1, int xNext = -1)
        => new DialogueEntry { kind = LineKind.Choice, choicePrompt = prompt, optionO = o, optionX = x, nextIndexIfO = oNext, nextIndexIfX = xNext };
}

