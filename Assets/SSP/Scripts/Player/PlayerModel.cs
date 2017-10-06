using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerModel : NetworkBehaviour, IHealth, IEther
{
    [SerializeField, SyncVar] public int playerId = 0;
    [SerializeField, SyncVar] public int teamId = 0;

    [SerializeField] private float devHealth;
    [SerializeField] private float devEther;
    public ReactiveProperty<float> Health = new ReactiveProperty<float>();
    public ReactiveProperty<float> Ether = new ReactiveProperty<float>();
    [SerializeField] private float initialHealth;
    [SerializeField] private float initialEther;

    [SerializeField] public bool isLocalPlayerCharacter = false;

    [HideInInspector] public int defaultLayer = LayerMap.Default;   //ネットワーク実装時にはローカルプレイヤーのみLayerMap.LocalPlayerになる。

    private void Start()
    {
        Health = new ReactiveProperty<float>();
        Ether = new ReactiveProperty<float>();

        if (playerId == 0) playerId = Random.Range(1, 100);
        if (teamId == 0) teamId = Random.Range(1, 100);
        this.ObserveEveryValueChanged(_ => devHealth).Subscribe(v => Health.Value = v);
        Health.Subscribe(v => devHealth = v);
        this.ObserveEveryValueChanged(_ => devEther).Subscribe(v => Ether.Value = v);
        Ether.Subscribe(v => devEther = v);

        Init();
    }

    public void Init()
    {
        Health.Value = initialHealth;
        Ether.Value = initialEther;
    }

    public float GetHealth()
    {
        return Health.Value;
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
