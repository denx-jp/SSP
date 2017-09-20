using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameJudger : MonoBehaviour {

    [SerializeField] private LifeSupportSystemModel aTeamLSSModel;
    [SerializeField] private LifeSupportSystemModel bTeamLSSModel;

	// Use this for initialization
	void Start () {
        aTeamLSSModel.ether.Subscribe(v=>Debug.Log("team A:"+v));
        bTeamLSSModel.ether.Subscribe(v=>Debug.Log("team B:"+v));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
