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
    private PlayerModel playerModel;
    private GameObject[] respawnPoints;

    [SerializeField] private int timeToRespawn;

    private Animator animator;
    private int deathHash = Animator.StringToHash("Death");

    void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        animator = GetComponent<Animator>();
        respawnPoints = GameObject.FindGameObjectsWithTag(TagMap.Respawn);

        this.playerHealthManager.GetDeathStream()
            .Throttle(TimeSpan.FromSeconds(timeToRespawn))
            .Where(v => v)
            .Subscribe(_ =>
            {
                CmdPlayerRespawnStart();
            });
    }

#if ONLINE
    [Command]
#endif
    private void CmdPlayerRespawnStart()
    {
        var respawnPoint = respawnPoints[UnityEngine.Random.Range(0, respawnPoints.Length)];
        RpcPlayerRespawnStart(respawnPoint.transform.position);
    }
#if ONLINE
    [ClientRpc]
#endif
    private void RpcPlayerRespawnStart(Vector3 _respawnPointPosition)
    {
        playerModel.Init();
        this.transform.position = _respawnPointPosition;
        animator.SetBool(deathHash, false);
    }
}
