using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour {

    [SerializeField] private LifeSupportSystemEtherManager teamALSSManager;
    [SerializeField] private LifeSupportSystemEtherManager teamBLSSManager;

    private Subject<int> judgeStream;
    private GameObject[] players;

	void Start () {
        judgeStream = new Subject<int>();
        players = GameObject.FindGameObjectsWithTag(TagMap.Player);

        Observable
            //.CombineLatest(teamALSSManager, teamBLSSManager)

        //Observable
        //    .ZipLatest(teamALSSManager.ether, bTeamLSSModel.ether)
        //    .Where(v => v[0] <= 0 || v[1] <= 0)
        //    .Subscribe(v =>
        //    {
        //        if (v[0] > v[1]) { judgeStream.OnNext(1); }
        //        else if (v[0] < v[1]) { judgeStream.OnNext(2); }
        //        else if (v[0] == 0 && v[1] == 0) { judgeStream.OnNext(3); }
        //    }).AddTo(this);

        judgeStream.Subscribe(vv => Debug.Log(vv));

	}
    public Subject<int> GetjudgeStream()
    {
        return judgeStream;
    }
}
