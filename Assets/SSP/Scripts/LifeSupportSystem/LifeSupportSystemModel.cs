using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LifeSupportSystemModel : MonoBehaviour
{
    [SerializeField] private int teamId;
    public ReactiveProperty<float> ether;
    //[SyncVar]
    [SerializeField] private float syncEther;

    private void Start()
    {
        ether = new ReactiveProperty<float>();

        ether.Subscribe(v => syncEther = v);
        this.ObserveEveryValueChanged(_ => syncEther).Subscribe(v => ether.Value = v);
    }
}
