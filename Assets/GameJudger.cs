using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour {

    [SerializeField] private LifeSupportSystemEtherManager team1_LSSManager;
    [SerializeField] private LifeSupportSystemEtherManager team2_LSSManager;
    [SerializeField] private List<PlayerResultNotifier> playerResultNotifiers;

    private Subject<int> judgeStream;
    private GameObject[] players;

	void Start () {
        judgeStream = new Subject<int>();
        players = GameObject.FindGameObjectsWithTag(TagMap.Player);

        Observable
            .Merge(team1_LSSManager.GetDeathStream(), team2_LSSManager.GetDeathStream())
            .Subscribe(v =>
            {
                
                Debug.Log(v);
            });

        players.Select(p => p.GetComponent<PlayerModel>().teamId);
    }
    public Subject<int> GetJudgeStream()
    {
        return judgeStream;
    }
}
