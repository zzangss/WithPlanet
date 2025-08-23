using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    // talkData[npcId][stage] = string[] (대사 배열)
    private readonly Dictionary<Type, Dictionary<State, string[]>> talkData = new Dictionary<Type, Dictionary<State, string[]>>();

    // 편의를 위한 NPC ID 상수 (ObjData.id에 맞춰주세요)
    public const int NPC_BOSS = 1000;
    public const int NPC_MILUNA = 1100;
    public const int NPC_BOXMONSTER = 1200;

    private void Awake()
    {
        GenerateData();
    }

    // 여기에 노션의 실제 대사로 채워넣으세요.
    // 각 string[]는 한 줄이 한 번의 Talk() 출력입니다.
    private void GenerateData()
    {
        // Boss
        talkData[Type.Boss] = new Dictionary<State, string[]>
        {
            [State.Opening] = new[]
            {
                // ▼▼ 여기에 Boss 오프닝 설명(게임 시작 직후 재생)을 넣으세요 ▼▼
                "Star, remember your mission here.\n1) Find out the most valuable treasures/collectibles on this planet.\n2) Collect them and recycle it with our machine.\nCan you get it?",
                "There’s plenty of collectibles here. They all have different values, but don't bring everything back. You have to select them by yourself.\nYou’ll figure out what kind of treasure is valuable by explore the story on this planet. ",
                "The collectibles can simply divided in 3 types.\n1) Common collectibles\n2) The Core collectible\n3) Other collectibles",
                "We only collect the Common&Core things. \nThe common collectibles can achieve the target value.\r\nBut if you can’t bring back the Core collectible, no matter if you’ve collected enough values of the collectibles you will fail the mission.",
                "Other collectibles also have their own values, but Be careful!!! It will count as minus when you finally recycle.",
                "So simply it will goes like this. Keep this rule in mind. ",
                "Now go get the Mission point. It will tells you how much values you have to collect. And maybe… you can find some clues about this planet."
            },
            [State.PreMinigame] = new[]
            {
                // ▼▼ 미니게임 시작 전 Boss 대사 ▼▼
                "."
            },
            [State.PostMinigame] = new[]
            {
                // ▼▼ 미니게임 후 Boss 대사(성공/실패 공통 또는 분기 처리 가능) ▼▼
                "."
            }
        };

        // Miluna
        talkData[Type.Miluna] = new Dictionary<State, string[]>
        {
            [State.PreMinigame] = new[]
            {
                "I never seen you before, how did you get here?",
                "I don't care what you're here for, I don't have time for you. \nThere are too many negative comments posted with my content, I have to fix them so I can manage with you, OK?",
                "Or… maybe, you can help me with it?",
                "Yes! it will helps a lot. \nYou have to delete all the negative comments under the video. \nAre you ready? Click the screen to start.",

            },
            [State.PostMinigame] = new[]
            {
                "Wow! you are awesome. Guess I need a professional like you to manage my account.\nBy the way, you were asking about the most valuable thing on our planet, right?",
                "I would say it's definitely those sweet things, like candy and desserts! \nThey keep us in a good mood and provide us with the energy to survive. I can't even imagine how to live without them.",
                "Go ahead, collect whatever you want, Smartie!"
                
            }
        };

        // BoxMonster
        talkData[Type.BoxMonster] = new Dictionary<State, string[]>
        {
            [State.PreMinigame] = new[]
            {
                "I never seen you before, how did you get here?",
                "I don't care what you're here for, I don't have time for you. \nThere are too many negative comments posted with my content, I have to fix them so I can manage with you, OK?",
                "Or… maybe, you can help me with it?",
                "Yes! it will helps a lot. \nYou have to delete all the negative comments under the video. \nAre you ready? Click the screen to start.",
                "Oh, don’t panic. I just need some help… ",
                "I knew that you came here for those collectibles.\nBut if you want to know which collectible is valuable, you must help the owner of this planet, MILUNA. ",
                "MILUNA have got in trouble with the career as a popular influencer. You need to help MILUNA get through this rough period.",
                "Come on. I’ll take you to MILUNA’s room. You can find your mission point there."
            },
            [State.PostMinigame] = new[]
            {
                "Oh, You did it!"
            }
        };
    }

    // talkIndex가 범위를 넘으면 null 반환
    public string GetTalk(Type npcId, State stage, int talkIndex)
    {
        if (!talkData.TryGetValue(npcId, out var stageMap)) return null;
        if (!stageMap.TryGetValue(stage, out var lines)) return null;

        if (talkIndex < 0 || talkIndex >= lines.Length) return null;
        return lines[talkIndex];
    }
}
