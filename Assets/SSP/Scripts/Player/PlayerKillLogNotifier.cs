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
        playerHealthManager = GetComponent<PlayerHealthManager>();
        var myId = this.transform.GetComponentInParent<PlayerModel>().playerId;

        playerHealthManager.GetDeathStream()
            .Subscribe(_ =>
            {
                killLogStream.OnNext(new KeyValuePair<int, int>(playerHealthManager.recentAttackerId, myId));
            });
    }

    public Subject<KeyValuePair<int, int>> GetKillLogStream()
    {
        return killLogStream;
    }
}
