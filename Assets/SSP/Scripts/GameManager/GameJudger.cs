using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour
{
    [SerializeField] private LifeSupportSystemEtherManager team1_LSSManager;
    [SerializeField] private LifeSupportSystemEtherManager team2_LSSManager;

    private Subject<bool> judgeStream = new Subject<bool>();

    void Init()
    {
        Observable
            .Merge(team1_LSSManager.GetDeathStream(), team2_LSSManager.GetDeathStream())
            .Subscribe(v =>
            {
                // 敗北したチームのidを勝利したチームのidに変換
                // v = 1 -> 2, v = 2 -> 1
                int winnerTeamId = v * 2 % 3;

                PlayerManager localPlayerManager = ClientPlayersManager.Players.Find(p => p.playerModel.isLocalPlayerCharacter);
                int localPlayerTeamId = localPlayerManager.playerModel.teamId;
                bool isWin = winnerTeamId == localPlayerTeamId;
                judgeStream.OnNext(isWin);
            }).AddTo(gameObject);
    }

    public Subject<bool> GetJudgeStream()
    {
        return judgeStream;
    }
}
