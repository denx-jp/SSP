using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;
using System.Linq;
public class PlayerRespawner : NetworkBehaviour
{
    int PointNum;
    private GameObject LSS;

    private PlayerHealthManager playerHealthManager;
    [SerializeField] private PlayerInputManager pim;
    private PlayerModel playerModel;
    private GameObject[] nearPoints;
    private GameObject[] respawnPoints;

    [SerializeField] private float Range;

    [SerializeField] private int timeToRespawn;

    private Animator animator;
    private int deathHash = Animator.StringToHash("Death");

    void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        animator = GetComponent<Animator>();
        //nearPoints = GameObject.FindGameObjectsWithTag(TagMap.Respawn);
        respawnPoints = GameObject.FindGameObjectsWithTag(TagMap.Respawn);

        this.playerHealthManager.GetDeathStream()
            .Throttle(TimeSpan.FromSeconds(timeToRespawn))
            .Where(v => v)
            .Where(_ => isLocalPlayer)
            .Subscribe(_ =>
            {
                for (int i = 0; i < respawnPoints.Length; i++)
                {
                    float Distance = (LSS.transform.position - respawnPoints[i].transform.position).sqrMagnitude;
                    var nearPoints = respawnPoints.Where( e => Distance < Range);   
                }
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
