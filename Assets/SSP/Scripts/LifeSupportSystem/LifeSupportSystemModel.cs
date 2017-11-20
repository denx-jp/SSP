using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Networking;

public class LifeSupportSystemModel : NetworkBehaviour, IEther
{
    public int teamId;
    public ReactiveProperty<float> ether = new ReactiveProperty<float>();
    [SyncVar] public float syncEther;
    [SerializeField] private float initEtherValue;

    private void Start()
    {
        this.ObserveEveryValueChanged(_ => syncEther).Subscribe(v => ether.Value = v).AddTo(gameObject);

        syncEther = initEtherValue;
        ether.Value = syncEther;
    }

    public float GetEther()
    {
        return ether.Value;
    }

    public float GetMaxEther()
    {
        return initEtherValue;
    }

    public ReactiveProperty<float> GetEtherStream()
    {
        return ether;
    }
}
