using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Networking;

public class LifeSupportSystemModel : NetworkBehaviour
{
    [SerializeField] private int teamId;
    public ReactiveProperty<float> ether = new ReactiveProperty<float>();
    [SyncVar, SerializeField] private float syncEther;
    [SerializeField] private float initEtherValue;

    private void Start()
    {
        ether.Subscribe(v => syncEther = v);
        this.ObserveEveryValueChanged(_ => syncEther).Subscribe(v => ether.Value = v);

        ether.Value = initEtherValue;
    }

    public int GetTeamId()
    {
        return teamId;
    }
}
