using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class OtherPlayerHPDisplayer : MonoBehaviour
{

    [SerializeField] private GameObject playerObj;

    private PlayerModel playerModel;
    private Transform playerCamera;

    void Start()
    {
        playerModel = playerObj.GetComponent<PlayerModel>();
        playerCamera = Camera.main.gameObject.transform;

        if (playerModel.isLocalPlayer)
            gameObject.SetActive(false);
    }

    void Update()
    {
        // 他プレイヤーのHPバーを自HUDに対して垂直に表示
        if (playerCamera != null)
            this.transform.rotation = playerCamera.rotation;
    }
}
