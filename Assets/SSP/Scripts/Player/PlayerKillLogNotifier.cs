using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerKillLogNotifier : MonoBehaviour
{
    private PlayerHealthManager playerHealthManager;
    private Subject<KeyValuePair<int, int>> killLogStream = new Subject<KeyValuePair<int, int>>();

    private void Start()
    {
        //killLogStream = new Subject<KeyValuePair<int, int>>();
        GetPlayerHealthManager().GetDeathStream()
            .Subscribe(_ =>
            {
                var myId = this.transform.GetComponentInParent<PlayerModel>().playerId;
                killLogStream.OnNext(new KeyValuePair<int, int>(GetPlayerHealthManager().recentAttackerId, myId));
            });
    }

    public Subject<KeyValuePair<int, int>> GetKillLogStream()
    {
        return killLogStream;
    }

    private PlayerHealthManager GetPlayerHealthManager()
    {
        if (playerHealthManager == null)
            playerHealthManager = GetComponent<PlayerHealthManager>();
        return playerHealthManager;
    }
}
