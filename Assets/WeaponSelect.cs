using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelect : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
        //F（左）とG（右）で武器選択
        if ((Input.GetKeyDown(KeyCode.F)) && (this.transform.position.x >= 50)) 
        {
            this.transform.position += new Vector3(-46.3f, 0, 0);
        }
        if ((Input.GetKeyDown(KeyCode.G))&& (this.transform.position.x <=170)) 
        {
            this.transform.position += new Vector3(46.3f, 0, 0);
        }
    }
}
