using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerModel : MonoBehaviour, IHealth
{
    [SerializeField] public int playerId;
    //[SyncVar] ネットワーク実装の時、SerializeFieldからSyncVarに変更
    [SerializeField] private float syncHealth;
    //[SyncVar]
    [SerializeField] private float syncEther;
    public ReactiveProperty<float> Health = new ReactiveProperty<float>();
    public ReactiveProperty<float> Ether = new ReactiveProperty<float>();

    private void Start()
    {
        Health = new ReactiveProperty<float>();
        Ether = new ReactiveProperty<float>();

        this.ObserveEveryValueChanged(_ => syncHealth)
            .Subscribe(v => Health.Value = v);
        Health.Subscribe(v => syncHealth = v);

        this.ObserveEveryValueChanged(_ => syncEther)
            .Subscribe(v => Ether.Value = v);
        Ether.Subscribe(v => syncEther = v);
    }

    public float GetHealth()
    {
        return Health.Value;
    }

    public ReactiveProperty<float> GetHealthStream()
    {
        return Health;
    }

}
