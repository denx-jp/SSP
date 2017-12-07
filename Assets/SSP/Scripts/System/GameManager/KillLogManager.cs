using System.Collections.Generic;
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
                        RpcUpdatePlayerRecord(v.Key, v.Value);
                    }
                );
        }
    }

    public PlayerRecord GetPlayerRecord(int playerId)
    {
        return playerRecords[playerId];
    }

    [ClientRpc]
    void RpcUpdatePlayerRecord(int killerId, int victimId)
    {
        playerRecords[killerId].killCount++;
        playerRecords[victimId].deathCount++;
    }
}
