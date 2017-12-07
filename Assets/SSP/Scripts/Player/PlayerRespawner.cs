using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerRespawner : NetworkBehaviour
{
    private PlayerModel playerModel;
    private PlayerHealthManager playerHealthManager;
    private GameObject[] respawnPoints;

    [SerializeField] private int timeToRespawn;

    void Start()
    {
        playerModel = GetComponent<PlayerModel>();
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
        var respawnPos = SpawnPointManager.Instance.GetSpawnPositionAroundLSS(playerModel.teamId);
        RpcPlayerRespawnStart(respawnPos);
    }
    [ClientRpc]
    private void RpcPlayerRespawnStart(Vector3 respawnPointPosition)
    {
        this.transform.position = respawnPointPosition;
        playerHealthManager.Revive();
    }
}
