using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour
{
    [SerializeField] private LifeSupportSystemEtherManager team1LSS;
    [SerializeField] private LifeSupportSystemEtherManager team2LSS;

    private Subject<bool> judgeStream = new Subject<bool>();

    void Start()
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
