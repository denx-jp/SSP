using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject gameManager;

    public PlayerModel playerModel;
    public PlayerHealthManager playerHealthManager;
    public PlayerEtherManager playerEtherManager;
    public PlayerInputManager playerInputManager;
    public PlayerKillLogNotifier playerKillLogNotifier;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<ClientPlayersManager>().AddPlayer(this);
    }

    public void Init()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        playerEtherManager = GetComponent<PlayerEtherManager>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerKillLogNotifier = GetComponent<PlayerKillLogNotifier>();
    }
    
}
