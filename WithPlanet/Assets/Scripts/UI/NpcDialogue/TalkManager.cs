using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    // talkData[npc][stage] = DialogueEntry[]
    private readonly Dictionary<Type, Dictionary<State, DialogueEntry[]>> talkData
        = new Dictionary<Type, Dictionary<State, DialogueEntry[]>>();

    private void Awake() => GenerateData();

    private void GenerateData()
    {
        // Boss
        talkData[Type.Boss] = new Dictionary<State, DialogueEntry[]>
        {
            [State.Opening] = new[]
            {
                DialogueEntry.Line("Star, remember your mission here.\n1) Find out the most valuable treasures/collectibles on this planet.\n2) Collect them and recycle it with our machine.\nCan you get it?"),
                DialogueEntry.Line("There’s plenty of collectibles here..."),
                DialogueEntry.Line("The collectibles can simply divided in 3 types.\n1) Common collectibles\n2) The Core collectible\n3) Other collectibles"),
                DialogueEntry.Line("We only collect the Common&Core things..."),
                DialogueEntry.Line("Other collectibles also have their own values, but Be careful!!! ..."),
                DialogueEntry.Line("So simply it will goes like this. Keep this rule in mind."),
                DialogueEntry.Line("Now go get the Mission point...")
            },
            [State.PreMinigame] = new[] { DialogueEntry.Line(".") },
            [State.PostMinigame] = new[] { DialogueEntry.Line(".") },
        };

        // Miluna — 예시: 선택지 분기
        talkData[Type.Miluna] = new Dictionary<State, DialogueEntry[]>
        {
            [State.PreMinigame] = new[]
            {
                DialogueEntry.Line("I never seen you before, how did you get here?"),
                DialogueEntry.Line("I don't care what you're here for..."),
                DialogueEntry.Choice("Or… maybe, you can help me with it?", "O (help)", "X (reject)", oNext: 4, xNext: 5),
                DialogueEntry.Line("Yes! it will helps a lot. \nYou have to delete all the negative comments... Click the screen to start."), // idx 4 (O)
                DialogueEntry.Line("Hmm… It won’t take long. I really need your help..."), // idx 5 (X)
                DialogueEntry.Choice("Can you help me now?", "O (ok)", "X (sorry)", oNext: 4, xNext: 7),
                DialogueEntry.Line("Alright… come back if you change your mind."), // idx 7 (거절 종료)
            },
            [State.PostMinigame] = new[]
            {
                DialogueEntry.Line("Wow! you are awesome..."),
                DialogueEntry.Line("I would say it's definitely those sweet things..."),
                DialogueEntry.Line("Go ahead, collect whatever you want, Smartie!")
            }
        };

        // BoxMonster
        talkData[Type.BoxMonster] = new Dictionary<State, DialogueEntry[]>
        {
            [State.PreMinigame] = new[]
            {
                DialogueEntry.Line("Oh, don’t panic. I just need some help… "),
                DialogueEntry.Line("I knew that you came here for those collectibles..."),
                DialogueEntry.Line("MILUNA have got in trouble..."),
                DialogueEntry.Line("Come on. I’ll take you to MILUNA’s room...")
            },
            [State.PostMinigame] = new[] { DialogueEntry.Line("Oh, You did it!(box)") }
        };
    }

    // talkIndex 범위 밖이면 null
    public DialogueEntry GetTalk(Type npc, State stage, int talkIndex)
    {
        if (!talkData.TryGetValue(npc, out var stageMap)) return null;
        if (!stageMap.TryGetValue(stage, out var lines)) return null;
        if (talkIndex < 0 || talkIndex >= lines.Length) return null;
        return lines[talkIndex];
    }
}
