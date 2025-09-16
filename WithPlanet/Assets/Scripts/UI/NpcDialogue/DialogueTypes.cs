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
    public int nextIndexIfO = -1;
    public int nextIndexIfX = -1;

    // ★ 추가: 선택 직후 '다음 한 줄'을 보여주고 곧바로 종료할지 여부
    public bool endAfterSelectO = false;
    public bool endAfterSelectX = false;

    public static DialogueEntry Line(string t) => new DialogueEntry { kind = LineKind.Text, text = t };

    public static DialogueEntry Choice(
        string prompt, string o = "O", string x = "X",
        int oNext = -1, int xNext = -1,
        bool endAfterO = false, bool endAfterX = false)
    {
        return new DialogueEntry
        {
            kind = LineKind.Choice,
            choicePrompt = prompt,
            optionO = o,
            optionX = x,
            nextIndexIfO = oNext,
            nextIndexIfX = xNext,
            endAfterSelectO = endAfterO,
            endAfterSelectX = endAfterX
        };
    }
}
