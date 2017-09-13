using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerRespawner : MonoBehaviour
{

    private PlayerHealthManager playerHealthManager;
    private PlayerModel playerModel;

    // リスポーンするまでの時間
    [SerializeField] private int timeToRespawn;
    // スポナー
    [SerializeField] private GameObject respawner;

    // Use this for initialization
    void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();

        this.UpdateAsObservable()
            .Where(_ => !playerHealthManager.IsAlive())
            .First()
            .Subscribe(_ =>
            {
                PlayerRespawn();
                //				RespawnTimer();
            });
    }

    void PlayerRespawn()
    {
        Observable.Timer(TimeSpan.FromSeconds(timeToRespawn))
            .Subscribe(_ =>
            {
                this.transform.position = respawner.transform.position;
            });
    }

    //	void RespawnTimer(){
    //		Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (1))
    //			.Select (x => timeToRespawn - x)
    //			.TakeWhile (x => x > 0)
    //			.Subscribe (l => {
    //			Debug.Log ("リスポーンまであと" + l + "秒");
    //		});
    //	}
}
