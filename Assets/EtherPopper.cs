using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
public class EtherPopper : MonoBehaviour {

	//float delta; //時間経過
	public GameObject Ether;
	public GameObject Point;
	[SerializeField] private float Span; // pop間の時間設定
	[SerializeField] private List<GameObject> popPoints = new List<GameObject>();	//ポップ地点の管理

	void Start () {
		Observable.Interval(TimeSpan.FromSeconds(Span)).Subscribe(l => 
			{
				GameObject ether = Instantiate (Ether) as GameObject;
				Point = popPoints [UnityEngine.Random.Range (0, popPoints.Count)]; 
				ether.transform.position = Point.transform.position;
		}).AddTo (this);
	
	}

	void Update () {
		//this
		//if (this.delta > this.Span) {
		//	this.delta = 0;
		//
		//}
	}
}
