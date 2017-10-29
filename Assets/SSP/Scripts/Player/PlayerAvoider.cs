﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerAvoider : NetworkBehaviour
{
    private int avoidHash = Animator.StringToHash("RollForward");
    private Animator animator;
    private AnimatorStateInfo state;
    private PlayerInputManager pim;
    private PlayerModel playerModel;

    [SerializeField] private float avoidStartTime = 0.1f;
    [SerializeField] private float avoidDuration = 0.3f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();
        playerModel = GetComponent<PlayerModel>();

        this.UpdateAsObservable()
            .Subscribe(_ => state = animator.GetCurrentAnimatorStateInfo(0));

        pim.AvoidButtonDown
            .Where(v => v)
            .Where(_ => state.shortNameHash != avoidHash)
            .Subscribe(v =>
            {
                CmdAvoiding();
            });
    }
    
    [Command]
    private void CmdAvoiding()
    {
        RpcAvoiding();
    }
    
    [ClientRpc]
    private void RpcAvoiding()
    {
        StartCoroutine(Avoiding());
    }

    private IEnumerator Avoiding()
    {
        animator.SetTrigger(avoidHash);
        yield return new WaitForSeconds(avoidStartTime);
        SetLayer(LayerMap.Invincible);
        yield return new WaitForSeconds(avoidDuration);
        SetLayer(playerModel.defaultLayer);
        SetLayer(LayerMap.LocalPlayer);
    }

    private void SetLayer(int layer)
    {
        this.gameObject.layer = layer;
    }
}
