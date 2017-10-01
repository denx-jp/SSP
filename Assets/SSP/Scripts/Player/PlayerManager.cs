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

    [SerializeField] private bool devIsLocalPlayerCharacter = false;     //デバッグ用フラグ。OFFLINE環境のときtrueの場合のみLocalPlayerに指定される。

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#if ONLINE
        if (isLocalPlayer)
        {
            playerModel.isLocalPlayerCharacter = true;
            
            playerInputManager.enabled = true;
            playerCameraController.enabled = true;
            playerModel.defaultLayer = LayerMap.LocalPlayer;
            this.gameObject.layer = LayerMap.LocalPlayer;
        }
#else
        if (devIsLocalPlayerCharacter)
        {
            this.gameObject.layer = LayerMap.LocalPlayer;
            playerModel.isLocalPlayerCharacter = true;
        }
#endif
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

    public T GetPlayerComponent<T>()
    {
        if (typeof(T) == typeof(PlayerModel))
            return (T)(object)playerModel;
        return default(T);
    }

}
