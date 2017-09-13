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

    [SerializeField] private int timeToRespawn;
    [SerializeField] private GameObject respawnPoint;

	private Animator animator;
	private int deathHash = Animator.StringToHash("Death");

    void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
		animator = GetComponent<Animator>();

		this.playerHealthManager.GetDeathStream ()
			.Throttle (TimeSpan.FromSeconds (timeToRespawn))
            .Where (_ => !playerHealthManager.IsAlive ())
            .First ()
            .Subscribe (_ => {
				animator.SetBool(deathHash, false);
				this.transform.position=respawnPoint.transform.position;
			});
    }
}
