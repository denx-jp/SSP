using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerModel : MonoBehaviour
{

    public int id;
    public ReactiveProperty<float> Health = new ReactiveProperty<float>();
    public ReactiveProperty<float> Ether = new ReactiveProperty<float>();

    //デバッグ用（インスペクタで値をみるため）
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentEther;

    private void Start()
    {
        Health.Subscribe(v => currentHealth = v);
        Ether.Subscribe(v => currentEther = v);
    }
}
