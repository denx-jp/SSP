using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerRespawner : NetworkBehaviour
{
    private PlayerHealthManager playerHealthManager;
    private GameObject[] respawnPoints;

    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private int timeToRespawn;

    void Start()
    {
        playerHealthManager = GetComponent<PlayerHealthManager>();
        respawnPoints = GameObject.FindGameObjectsWithTag(TagMap.Respawn);

        this.playerHealthManager.GetDeathStream()
            .Throttle(TimeSpan.FromSeconds(timeToRespawn))
            .Where(v => v)
            .Where(_ => isLocalPlayer)
            .Subscribe(_ =>
            {
                CmdPlayerRespawnStart("inGame");
            });
    }

    [Command]
    public void CmdPlayerRespawnStart(string _state)
    {
        var teamId = playerModel.teamId;
        var respawnPoint = SpawnablePositionManager.Instance.GetSpawnPosition(teamId);

        RpcPlayerRespawnStart(respawnPoint.position, _state);
    }

    [ClientRpc]
    private void RpcPlayerRespawnStart(Vector3 _respawnPointPosition, string _state)
    {
        this.transform.position = _respawnPointPosition;

        if(_state == "inGame")
            playerHealthManager.Revive();
    }
}
