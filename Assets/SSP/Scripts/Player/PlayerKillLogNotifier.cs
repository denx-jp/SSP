using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Networking;

public class PlayerKillLogNotifier : NetworkBehaviour
{
    private PlayerHealthManager playerHealthManager;
    private Subject<KeyValuePair<int, int>> killLogStream = new Subject<KeyValuePair<int, int>>();
    private int myId;

    private void Start()
    {
        playerHealthManager = GetComponent<PlayerHealthManager>();
        myId = this.transform.GetComponentInParent<PlayerModel>().playerId;

        playerHealthManager.GetDeathStream()
            .Subscribe(_ =>
            {
                if (isLocalPlayer)
                {
                    CmdPlayerKilled();
                }
            });
    }

    public Subject<KeyValuePair<int, int>> GetKillLogStream()
    {
        return killLogStream;
    }

    [Command]
    void CmdPlayerKilled()
    {
        RpcPlayerKilled();
    }

    [ClientRpc]
    void RpcPlayerKilled()
    {
        killLogStream.OnNext(new KeyValuePair<int, int>(playerHealthManager.recentAttackerId, myId));
    }
}
