using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class OtherPlayerHPDisplayer : MonoBehaviour {

    private GameObject playerCamera;

	void Start ()
    {
        playerCamera = Camera.main.gameObject;
    }

    void Update ()
    {
        // 他プレイヤーのHPバーを自HUDに対して垂直に表示
        if (playerCamera != null)
            this.transform.rotation = playerCamera.transform.rotation;
    }
}
