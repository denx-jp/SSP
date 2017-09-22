using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour {

    [SerializeField] private LifeSupportSystemEtherManager team1_LSSManager;
    [SerializeField] private LifeSupportSystemEtherManager team2_LSSManager;

    private Subject<int> winnerStream;

    void Awake()
    {
        winnerStream = new Subject<int>();
    }

	void Start ()
    {
        Observable
            .Merge(team1_LSSManager.GetDeathStream(), team2_LSSManager.GetDeathStream())
            .Subscribe(v =>
            {
                winnerStream.OnNext(v * 2 % 3);
            });
    }
    public Subject<int> GetWinnerStream()
    {
        return winnerStream;
    }
}
