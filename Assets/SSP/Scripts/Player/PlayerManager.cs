using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerManager : NetworkBehaviour
{
    public PlayerModel playerModel;
    public PlayerHealthManager playerHealthManager;
    public PlayerEtherManager playerEtherManager;
    public PlayerInputManager playerInputManager;
    public PlayerKillLogNotifier playerKillLogNotifier;
    public PlayerCameraController playerCameraController;
    public PlayerInventoryManager playerInventoryManager;

    private void Start()
    {
        ClientPlayersManager.AddPlayer(this);
        if (isLocalPlayer)
        {
            playerModel.isLocalPlayerCharacter = true;
            playerModel.defaultLayer = LayerMap.LocalPlayer;
            this.gameObject.layer = LayerMap.LocalPlayer;
        }
    }

    public void Init()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        playerEtherManager = GetComponent<PlayerEtherManager>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerKillLogNotifier = GetComponent<PlayerKillLogNotifier>();
        playerCameraController = GetComponent<PlayerCameraController>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
    }
}
