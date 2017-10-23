using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerRespawner : NetworkBehaviour
{
    int PointNum;
    private PlayerHealthManager playerHealthManager;
    [SerializeField] private PlayerInputManager pim;
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
            .Where(_ => isLocalPlayer)
            .Subscribe(_ =>
            {
                CmdPlayerRespawnStart();
            });

        pim.ChooseRespawnPointsRightButtonDown
            .Where(v => v)
            .Where(v => PointNum != respawnPoints.Length)
            .Subscribe(v =>
            {
                PointNum++;
                GetComponent<PlayerCameraController>().SetTarget(respawnPoints[PointNum]);
            });
        pim.ChooseRespawnPointsLeftButtonDown
            .Where(v => v)
            .Where(v => PointNum != 0)
            .Subscribe(v =>
            {
                PointNum--;
                GetComponent<PlayerCameraController>().SetTarget(respawnPoints[PointNum]);
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
        playerModel.Init();
        this.transform.position = _respawnPointPosition;
        animator.SetBool(deathHash, false);
    }
}
