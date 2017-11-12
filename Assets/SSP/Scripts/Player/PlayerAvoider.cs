using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerAvoider : NetworkBehaviour
{
    private PlayerInputManager pim;
    private PlayerModel playerModel;
    private PlayerAnimationController animationController;

    [SerializeField] private float avoidStartTime = 0.1f;
    [SerializeField] private float avoidDuration = 0.3f;

    private void Start()
    {
        pim = GetComponent<PlayerInputManager>();
        playerModel = GetComponent<PlayerModel>();
        animationController = GetComponent<PlayerAnimationController>();

        pim.AvoidButtonDown
            .Where(v => v)
            .Where(_ => playerModel.MoveMode == MoveMode.battle)
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
        animationController.Avoid();
        yield return new WaitForSeconds(avoidStartTime);
        SetLayer(LayerMap.Invincible);
        yield return new WaitForSeconds(avoidDuration);
        SetLayer(playerModel.defaultLayer);
    }

    private void SetLayer(int layer)
    {
        this.gameObject.layer = layer;
    }
}
