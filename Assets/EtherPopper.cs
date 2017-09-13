using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherPopper : MonoBehaviour {

	float delta; //時間経過
	public GameObject Ether;
	[SerializeField] private float TimeSpan; // pop間の時間設定
	[SerializeField] private List<GameObject> Area = new List<GameObject>();	//ポップ地点の管理
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.delta += Time.deltaTime;
		if (this.delta > this.TimeSpan) {
			this.delta = 0;
			GameObject ether = Instantiate (Ether) as GameObject;
			float px = Random.Range (-1, 1);
			float py = Random.Range (-1, 1);
			float pz = Random.Range (-1, 1);
			ether.transform.position = new Vector3 (px, py, pz);
		}
	}
}
