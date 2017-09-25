using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour
{
    [SerializeField] private ClientPlayersManager clientPlayersManager;
    [SerializeField] private LifeSupportSystemEtherManager team1_LSSManager;
    [SerializeField] private LifeSupportSystemEtherManager team2_LSSManager;

    private int localPlayerTeamId;

    private Subject<int> winnerStream = new Subject<int>();
    private Subject<int> loserStream = new Subject<int>();

    void Start()
    {
        Observable
            .Merge(team1_LSSManager.GetDeathStream(), team2_LSSManager.GetDeathStream())
            .Subscribe(v =>
            {
                // 敗北したチームのidを勝利したチームのidに変換
                // v = 1 -> 2, v = 2 -> 1
                int winnerTeamId = v * 2 % 3;

                PlayerManager localPlayerManager = clientPlayersManager.playerManagers.Find(p => p.gameObject.layer == LayerMap.LocalPlayer);
                int localPlayerId = localPlayerManager.playerModel.teamId;

                if (winnerTeamId == localPlayerTeamId)
                {
                    Debug.Log("Player" + localPlayerManager.playerModel.playerId +
                                "(Team" + localPlayerManager.playerModel.teamId + ")" + " :勝利");
                    winnerStream.OnNext(localPlayerId);
                }
                else
                {
                    Debug.Log("Player" + localPlayerManager.playerModel.playerId +
                                "(Team" + localPlayerManager.playerModel.teamId + ")" + " :敗北");
                    loserStream.OnNext(localPlayerId);
                }
            });
    }

    public Subject<int> GetWinnerStream()
    {
        return winnerStream;
    }

    public Subject<int> GetLoserStream()
    {
        return loserStream;
    }
}
