using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour {

    [SerializeField] private LifeSupportSystemModel aTeamLSSModel;
    [SerializeField] private LifeSupportSystemModel bTeamLSSModel;

    private float aTeamEther;
    private float bTeamEther;

    public Subject<int> judgeStream;

	// Use this for initialization
	void Start () {
        judgeStream = new Subject<int>();

        aTeamLSSModel.ether
            .Where(v => v <= 0)
            .Subscribe(v => judgeStream.OnNext(2));

        bTeamLSSModel.ether
            .Where(v => v <= 0)
            .Subscribe(v => judgeStream.OnNext(1));

        judgeStream.Subscribe(vv => Debug.Log(vv));

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
