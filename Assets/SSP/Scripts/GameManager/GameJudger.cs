using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour
{
    private Subject<bool> judgeStream = new Subject<bool>();

    public void Init(LifeSupportSystemEtherManager team1LSS, LifeSupportSystemEtherManager team2LSS)
    {
        Observable
            .Merge(team1LSS.GetDeathStream(), team2LSS.GetDeathStream())
            .Subscribe(deathLSSTeamId =>
            {
                PlayerManager localPlayerManager = ClientPlayersManager.Players.Find(p => p.playerModel.isLocalPlayerCharacter);
                int localPlayerTeamId = localPlayerManager.playerModel.teamId;
                bool isWin = deathLSSTeamId != localPlayerTeamId;
                judgeStream.OnNext(isWin);
            }).AddTo(gameObject);
    }

    public Subject<bool> GetJudgeStream()
    {
        return judgeStream;
    }
}
