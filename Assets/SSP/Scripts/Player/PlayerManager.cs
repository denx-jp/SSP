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
    public PlayerCameraController playerCameraController;

    private void Start()
    {
        if (isLocalPlayer)
        {
            playerInputManager.enabled = true;
            playerCameraController.enabled = true;
        }
    }

    public void Init()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        playerEtherManager = GetComponent<PlayerEtherManager>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerCameraController = GetComponent<PlayerCameraController>();
    }

}
