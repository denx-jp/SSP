using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerModel : MonoBehaviour, IHealth, IEther
{
    [SerializeField] public int playerId;
    [SerializeField] public int teamId;

    //[SyncVar] ネットワーク実装の時、SerializeFieldからSyncVarに変更
    [SerializeField] private float syncHealth;
    //[SyncVar]
    [SerializeField] private float syncEther;
    public ReactiveProperty<float> Health = new ReactiveProperty<float>();
    public ReactiveProperty<float> Ether = new ReactiveProperty<float>();

    [SerializeField] private float initialHealth;
    [SerializeField] private float initialEther;
    [HideInInspector] public int defaultLayer = LayerMap.Default;   //ネットワーク実装時にはローカルプレイヤーのみLayerMap.LocalPlayerになる。

    private void Start()
    {
        Health = new ReactiveProperty<float>();
        Ether = new ReactiveProperty<float>();

        this.ObserveEveryValueChanged(_ => syncHealth).Subscribe(v => Health.Value = v);
        Health.Subscribe(v => syncHealth = v);
        this.ObserveEveryValueChanged(_ => syncEther).Subscribe(v => Ether.Value = v);
        Ether.Subscribe(v => syncEther = v);

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
