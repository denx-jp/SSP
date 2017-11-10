using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using UniRx;

public class KillLogManager : NetworkBehaviour
{

    private Dictionary<int, PlayerRecord> playerRecords = new Dictionary<int, PlayerRecord>();

    public void Init()
    {
        playerRecords = ClientPlayersManager.Players.ToDictionary(v => v.playerModel.Id, _ => new PlayerRecord());

        if (isServer)
        {
            var killLogNotifiers = ClientPlayersManager.Instance.GetPlayersComponent<PlayerKillLogNotifier>();
            var allKillLogStream = Observable.Merge(killLogNotifiers.Select(v => v.GetKillLogStream()))
                .Subscribe(v =>
                    {
                        RpcUpdatePlayerReport(v.Key, v.Value);
                    }
                );
        }
    }

    [ClientRpc]
    void RpcUpdatePlayerReport(int killerId, int victimId)
    {
        playerRecords[killerId].killCount++;
        playerRecords[victimId].deathCount++;

        ReportKillStats();//デバッグ用　リザルト画面のVが出来たら消す
    }

    void ReportKillStats()
    {
        foreach (var record in playerRecords)
        {
            Debug.Log(string.Format("Plaryer{0} K:{1} D:{2}", record.Key, record.Value.killCount, record.Value.deathCount));
        }
    }
}
