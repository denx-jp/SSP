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
        var respawnPoint = respawnPoints[UnityEngine.Random.Range(0, respawnPoints.Length)];
        RpcPlayerRespawnStart(respawnPoint.transform.position);
    }

    [ClientRpc]
    private void RpcPlayerRespawnStart(Vector3 _respawnPointPosition)
    {
        this.transform.position = _respawnPointPosition;
        playerHealthManager.Revive();
    }
}
