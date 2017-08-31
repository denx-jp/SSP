using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EtherModel : MonoBehaviour
{
    [SerializeField] private float initialEther;
    [SerializeField] private float currentEther;

    public ReactiveProperty<float> Ether;

    protected virtual void Init()
    {
        Ether = new ReactiveProperty<float>();
        Ether.Value = initialEther;
    }

    protected virtual void Init(float etherValue)
    {
        Ether = new ReactiveProperty<float>();
        Ether.Value = etherValue;
    }

}
