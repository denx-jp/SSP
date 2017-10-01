using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerRespawner : MonoBehaviour
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
                playerModel.Init();
                var respawnPoint = respawnPoints[UnityEngine.Random.Range(0, respawnPoints.Length)];
                this.transform.position = respawnPoint.transform.position;
                animator.SetBool(deathHash, false);
            });
    }
}
