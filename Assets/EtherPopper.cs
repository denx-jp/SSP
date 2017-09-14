using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherPopper : MonoBehaviour {

	float delta; //時間経過
	public GameObject Ether;
	public GameObject Point;
	[SerializeField] private float TimeSpan; // pop間の時間設定
	[SerializeField] private List<GameObject> popPoints = new List<GameObject>();	//ポップ地点の管理

	void Start () {
		
	}

	void Update () {
		this.delta += Time.deltaTime;
		if (this.delta > this.TimeSpan) {
			this.delta = 0;
			GameObject ether = Instantiate (Ether) as GameObject;
			Point = popPoints [Random.Range (0, popPoints.Count)]; 
			ether.transform.position = Point.transform.position;
		}
	}
}
