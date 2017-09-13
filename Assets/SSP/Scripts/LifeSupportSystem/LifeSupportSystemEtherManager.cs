using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LifeSupportSystemEtherManager : MonoBehaviour, IInteractable
{
    [SerializeField] private float initEther;
    [SerializeField] private float etherReductionRate;
    [SerializeField] private float etherChargeValue;
    private LifeSupportSystemModel lifeSupportSystemModel;

    void Start()
    {
        lifeSupportSystemModel = GetComponent<LifeSupportSystemModel>();
        lifeSupportSystemModel.ether.Value = initEther;

        Observable.Interval(TimeSpan.FromMilliseconds(1000))
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
        playerManager.playerEtherManager.EmitEther(etherChargeValue);
        AcquireEther(etherChargeValue);
    }
}
