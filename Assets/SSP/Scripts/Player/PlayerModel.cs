﻿using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerModel : NetworkBehaviour, IHealth, IEther
{
    [SyncVar] public int Id = 0;
    [SyncVar] public string Name = "えすぴこ";
    [SyncVar] public int teamId = 0;
    [SyncVar] public float syncHealth;
    [SyncVar] public float syncEther;
    public ReactiveProperty<float> Health { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> Ether { get; private set; } = new ReactiveProperty<float>();
    [SerializeField] private float initialHealth;
    [SerializeField] private float initialEther;

    [SerializeField] public bool isLocalPlayerCharacter = false;
    //ネットワーク実装時にはローカルプレイヤーのみLayerMap.LocalPlayerになる。
    [HideInInspector] public int defaultLayer = LayerMap.Default;

    private void Awake()
    {
        syncEther = initialEther;
        Ether.Value = syncEther;

        this.ObserveEveryValueChanged(_ => syncHealth).Subscribe(v => Health.Value = v);
        this.ObserveEveryValueChanged(_ => syncEther).Subscribe(v => Ether.Value = v);

        Init();
    }

    public void Init()
    {
        syncHealth = initialHealth;
        Health.Value = syncHealth;
    }

    public float GetMaxHealth()
    {
        return initialHealth;
    }

    public ReactiveProperty<float> GetHealthStream()
    {
        return Health;
    }

    public bool IsAlive()
    {
        return Health.Value > 0.0f;
    }

    public float GetEther()
    {
        return Ether.Value;
    }

    public ReactiveProperty<float> GetEtherStream()
    {
        return Ether;
    }
}
