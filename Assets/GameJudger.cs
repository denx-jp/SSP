using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour {

    [SerializeField] private LifeSupportSystemModel aTeamLSSModel;
    [SerializeField] private LifeSupportSystemModel bTeamLSSModel;

    private Subject<int> judgeStream;

	void Start () {
        judgeStream = new Subject<int>();

        Observable
            .ZipLatest(aTeamLSSModel.ether, bTeamLSSModel.ether)
            .Where(v => v[0] <= 0 || v[1] <= 0)
            .Subscribe(v =>
            {
                if (v[0] > v[1]) { judgeStream.OnNext(1); }
                else if (v[0] < v[1]) { judgeStream.OnNext(2); }
                else if (v[0] == 0 && v[1] == 0) { judgeStream.OnNext(3); }
            }).AddTo(this);

        judgeStream.Subscribe(vv => Debug.Log(vv));

	}
    public Subject<int> GetjudgeStream()
    {
        return judgeStream;
    }
}
