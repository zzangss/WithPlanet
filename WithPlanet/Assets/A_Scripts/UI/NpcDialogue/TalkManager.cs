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
        };

        // Miluna — 예시: 선택지 분기
        talkData[Type.Miluna] = new Dictionary<State, DialogueEntry[]>
        {
            [State.PreMinigame1] = new[]
            {
                DialogueEntry.Line("I never seen you before, how did you get here?"),
                DialogueEntry.Line("I don't care what you're here for, I don't have time for you. \nThere are too many negative comments posted with my content, I have to fix them so I can manage with you, OK?"),
                DialogueEntry.Choice("Or… maybe, you can help me with it?", "O", "X", oNext: 5, xNext: 3, endAfterO: true, endAfterX:false ),
                DialogueEntry.Line("Hmm… It won’t take long. I really need your help..."), // idx 3 
                DialogueEntry.Choice("Can you help me now?", "O", "X", oNext: 5, xNext: 6),
                DialogueEntry.Line("Yes! it will helps a lot. \nYou have to delete all the negative comments under the video. \nAre you ready? Click the screen to start."), // idx 5
                DialogueEntry.Line("Oh, that's a pity."), // idx 6
            },
            [State.PreMinigame1_X] = new[]
            {
                DialogueEntry.Choice("Uh-huh…it’s you again. Are you back to help me?", "O", "X", oNext: 1, xNext: 2, endAfterO: true, endAfterX: true),
                DialogueEntry.Line("Yes! it will helps a lot. \nYou have to delete all the negative comments under the video. \nAre you ready? Click the screen to start."),
                DialogueEntry.Line("Oh, that's pity"),
            },
            [State.PreMinigame1_O] = new[]
            {
                DialogueEntry.Line("Are you ready? Click the screen to start.")
            },
            [State.PostMinigame1Fail] = new[]
            { 
                DialogueEntry.Line("Why are you here? You haven't fixed all these messes yet.\nDo not even think about the reward!")
            },
            [State.PostMinigame1Success] = new[]
            {
                DialogueEntry.Line("Wow! you are awesome. Guess I need a professional like you to manage my account."),
                DialogueEntry.Line("By the way, you were asking about the most valuable thing on our planet, right?"),
                DialogueEntry.Line("I would say it's definitely those sweet things, like candy and desserts that are valuable!"),
                DialogueEntry.Line("They keep us in a good mood and provide us with the energy to survive. I can't even imagine how to live without them."),
                DialogueEntry.Line("Go ahead, collect whatever you want, Smartie!")
            }
        };

        // BoxMonster
        talkData[Type.BoxMonster] = new Dictionary<State, DialogueEntry[]>
        {
            [State.PreMinigame1] = new[]
            {
                DialogueEntry.Line("Oh, don’t panic. I just need some help… "),
                DialogueEntry.Line("I knew that you came here for those collectibles..."),
                DialogueEntry.Line("MILUNA have got in trouble..."),
                DialogueEntry.Line("Come on. I’ll take you to MILUNA’s room...")
            },
            [State.PostMinigame1Success] = new[] { DialogueEntry.Line("Oh, You did it!(box)") }
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
