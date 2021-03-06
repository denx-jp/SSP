﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Networking;

public class LifeSupportSystemEtherManager : NetworkBehaviour, IInteractable, IDamageable
{
    private LifeSupportSystemModel lssModel;

    [SerializeField] private float etherReductionRate;           // 毎秒減少するエーテル量
    [SerializeField] private float etherChargeValue;              // インタラクトアクション1回で渡されるエーテル量
    [SerializeField] private GameObject etherObject;
    [SerializeField] private float emitPower;
    [SerializeField] private Vector3 emitDirectionRange;

    private Subject<int> deathStream = new Subject<int>();

    public void Init()
    {
        lssModel = GetComponent<LifeSupportSystemModel>();

        lssModel.ether
            .Where(v => v <= 0)
            .Subscribe(_ => deathStream.OnNext(lssModel.teamId));

        if (isServer)
        {
            Observable.Interval(System.TimeSpan.FromMilliseconds(1000))
                .Where(_ => lssModel.ether.Value > 0)
                .Subscribe(_ =>
                {
                    ReduceEther(etherReductionRate);
                }).AddTo(this);
        }
    }

    private void ReduceEther(float ether)
    {
        lssModel.syncEther -= ether;
    }

    private void AcquireEther(float ether)
    {
        lssModel.syncEther += ether;
    }

    public void Interact(PlayerManager playerManager)
    {
        if (playerManager.playerModel.teamId != lssModel.teamId) return;

        float playerEther = playerManager.playerModel.GetEther();

        if (playerEther - etherChargeValue > 0.0f)
        {
            playerManager.playerEtherManager.EmitEther(etherChargeValue);
            AcquireEther(etherChargeValue);
        }
        else if (playerEther > 0.0f)
        {
            playerManager.playerEtherManager.EmitEther(playerEther);
            AcquireEther(playerEther);
        }
    }

    public bool CanInteract()
    {
        if (ClientPlayersManager.Instance.GetLocalPlayer() == null) return false;
        return lssModel.ether.Value > 0 && ClientPlayersManager.Instance.GetLocalPlayer().playerModel.teamId == lssModel.teamId;
    }

    public Subject<int> GetDeathStream()
    {
        return deathStream;
    }

    public void SetDamage(Damage damage)
    {
        if (damage.AttackerTeamId == lssModel.teamId || lssModel.ether.Value <= 0) return;

        var emitEtherValue = damage.amount;
        CmdGenerateEtherObject(emitEtherValue);
        ReduceEther(emitEtherValue);
    }

    [Command]
    private void CmdGenerateEtherObject(float emitEtherValue)
    {
        float emitHeight = transform.localScale.y / 2;
        var singleEtherValue = emitEtherValue / 10;
        while (emitEtherValue > 0)
        {
            var emittedEtherObject = Instantiate(etherObject, transform.position + Vector3.up * emitHeight, transform.rotation);

            if (emitEtherValue < singleEtherValue) singleEtherValue = emitEtherValue;
            emitEtherValue -= singleEtherValue;
            emittedEtherObject.GetComponent<EtherObject>().Init(singleEtherValue);

            emitHeight += emittedEtherObject.transform.localScale.y;
            var emitDirection = new Vector3(
                Random.Range(-emitDirectionRange.x, emitDirectionRange.x),
                Random.Range(emitDirectionRange.y / 2, emitDirectionRange.y),
                Random.Range(-emitDirectionRange.z, emitDirectionRange.z)).normalized;
            emittedEtherObject.GetComponent<Rigidbody>().velocity = emitDirection * emitPower;
            NetworkServer.SpawnWithClientAuthority(emittedEtherObject, NetworkServer.connections[0]);
        }
    }
}
