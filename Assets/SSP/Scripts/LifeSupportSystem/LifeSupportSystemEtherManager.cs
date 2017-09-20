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

    [SerializeField] private GameObject playerBat;
    [SerializeField] private GameObject etherObject;
    [SerializeField] private float LSSPopEther;
    [SerializeField] private float emitPower;

    [SerializeField] private WeaponAttacker weaponAttacker;

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

        gameObject.OnTriggerEnterAsObservable()
            .Where(v => v.gameObject == playerBat)
            .Where(l => weaponAttacker.isAttackStarted)
            .Subscribe(_ =>
            {
                ReduceEther(LSSPopEther);
                GenerateEtherObject(LSSPopEther);
            });
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

    private void GenerateEtherObject(float emitEtherValue){
        float LSSemithigh = 0.0f;
        var singleEtherValue = emitEtherValue / 10;
        while (emitEtherValue > 0)
        {
            var emittedEtherObject = Instantiate(etherObject, transform.position + Vector3.up * LSSemithigh, transform.rotation);

            if (emitEtherValue < singleEtherValue) singleEtherValue = emitEtherValue;

            emitEtherValue -= singleEtherValue;
            emittedEtherObject.GetComponent<EtherObject>().Init(singleEtherValue);
            LSSemithigh += emittedEtherObject.transform.localScale.y;

            var emitDirestion = Vector3.up + new Vector3(UnityEngine.Random.Range(-emitPower, emitPower), 0, UnityEngine.Random.Range(-emitPower, emitPower));
            emittedEtherObject.GetComponent<Rigidbody>().AddForce(emitDirestion, ForceMode.Impulse);
        }

    }
}
