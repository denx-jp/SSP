using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UniRx;

public class PlayerHPDisplayer : NetworkBehaviour
{
    private PlayerModel playerModel;
    private PlayerHealthManager playerHealthManager;
    private GameObject playerCamera;

    [SerializeField] private Slider playerHPBar;

	void Start () {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        playerCamera = GameObject.FindWithTag("MainCamera");

        playerHPBar.maxValue = playerModel.syncHealth;
        this.playerModel.Health
            .Subscribe(v => playerHPBar.value = v);

        // 他プレイヤーのHPバーを自HUDに対して垂直に表示
        if (playerCamera != null)
            this.ObserveEveryValueChanged(_ => playerCamera.transform.rotation)
                .Subscribe(v => playerHPBar.transform.rotation = v);

        if (isLocalPlayer)
            playerHPBar.gameObject.SetActive(false);
    }
}
