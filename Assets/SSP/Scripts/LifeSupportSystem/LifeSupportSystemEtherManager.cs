using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Networking;

public class LifeSupportSystemEtherManager : NetworkBehaviour , IInteractable, IDamageable
{
    [SerializeField] private float etherReductionRate;
    [SerializeField] private float etherChargeValue;

    [SerializeField] private GameObject etherObject;
    [SerializeField] private float emitPower;
    [SerializeField] private float emittingEtherCoefficient;

    private Subject<int> deathStream;
    private LifeSupportSystemModel lifeSupportSystemModel;

    void Start()
    {
        lifeSupportSystemModel = GetComponent<LifeSupportSystemModel>();

        lifeSupportSystemModel.ether
            .Where(v => v <= 0)
            .Subscribe(_ => GetDeathStream().OnNext(lifeSupportSystemModel.GetTeamId()));

        Observable.Interval(TimeSpan.FromMilliseconds(1000))
            .Where(_ => lifeSupportSystemModel.ether.Value > 0)
            .Subscribe(_ =>
            {
                ReduceEther(etherReductionRate);
            }).AddTo(this);
    }

    private void ReduceEther(float ether)
    {
        lifeSupportSystemModel.ether.Value -= ether;
    }

    private void AcquireEther(float ether)
    {
        lifeSupportSystemModel.ether.Value += ether;
    }

    public void Interact(PlayerManager playerManager)
    {
        if (playerManager.playerModel.teamId != lifeSupportSystemModel.GetTeamId()) return;

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
        return lifeSupportSystemModel.ether.Value > 0;
    }

    public void SetDamage(Damage damage)
    {
        if (damage.teamId == lifeSupportSystemModel.GetTeamId()) return;

        float LSSemithigh = 0.0f;
        float emittingEtherAmount = damage.amount * emittingEtherCoefficient;
        ReduceEther(emittingEtherAmount);
        var singleEtherValue = damage.amount;

        while (emittingEtherAmount > 0)
        {
            var emittedEtherObject = Instantiate(etherObject, transform.position + Vector3.up * LSSemithigh, transform.rotation);

            if (emittingEtherAmount < singleEtherValue) singleEtherValue = emittingEtherAmount;

            emittingEtherAmount -= singleEtherValue;
            emittedEtherObject.GetComponent<EtherObject>().Init(singleEtherValue);
            LSSemithigh += emittedEtherObject.transform.localScale.y;

            var emitDirestion = Vector3.up + new Vector3(UnityEngine.Random.Range(-emitPower, emitPower), 0, UnityEngine.Random.Range(-emitPower, emitPower));
            emittedEtherObject.GetComponent<Rigidbody>().AddForce(emitDirestion, ForceMode.Impulse);
        }
    }

    public Subject<int> GetDeathStream()
    {
        if (deathStream == null)
            deathStream = new Subject<int>();
        return deathStream;
    }
}
