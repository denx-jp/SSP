using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class OtherPlayerHPDisplayer : MonoBehaviour {

    private GameObject playerCamera;

	void Start () {
        playerCamera = Camera.main.gameObject;

        // 他プレイヤーのHPバーを自HUDに対して垂直に表示
        if (playerCamera != null)
            this.ObserveEveryValueChanged(_ => playerCamera.transform.rotation)
                .Subscribe(v => transform.rotation = v);
    }

    void Update () {
		
	}
}
