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
                CmdPlayerRespawnStart();
            });
    }

    [Command]
    private void CmdPlayerRespawnStart()
    {
        var teamId = playerModel.teamId;
        var respawnPoint = SpawnablePositionManager.Instance.GetSpawnPosition(teamId);

        RpcPlayerRespawnStart(respawnPoint.position);
    }
    [ClientRpc]
    private void RpcPlayerRespawnStart(Vector3 _respawnPointPosition)
    {
        this.transform.position = _respawnPointPosition;
        playerHealthManager.Revive();
    }

    [Command]
    public void CmdInitPlayerSpawnStart()
    {
        var teamId = playerModel.teamId;
        var respawnPoint = SpawnablePositionManager.Instance.GetSpawnPosition(teamId);

        RpcInitPlayerSpawnStart(respawnPoint.position);
    }
    [ClientRpc]
    private void RpcInitPlayerSpawnStart(Vector3 _spawnPointPosition)
    {
        this.transform.position = _spawnPointPosition;
    }
}
