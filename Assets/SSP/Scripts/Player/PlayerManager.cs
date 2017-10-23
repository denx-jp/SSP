using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (isLocalPlayer)
        {
            playerModel.isLocalPlayerCharacter = true;
            
            playerInputManager.enabled = true;
            playerCameraController.enabled = true;
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
