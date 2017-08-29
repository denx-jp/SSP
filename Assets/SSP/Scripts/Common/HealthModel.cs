using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class HealthModel : MonoBehaviour
{
    [SerializeField] private float initialHealth;
    [SerializeField] private float currentHealth;

    public ReactiveProperty<float> Health;

    protected virtual void Init()
    {
        Health = new ReactiveProperty<float>();
        Health.Value = initialHealth;

        Health.Subscribe(v => currentHealth = v);
    }

    public virtual void SetDamage(float dmgamount)
    {
        if (Health.Value > 0.0f && dmgamount > 0.0f)
        {
            Health.Value -= dmgamount;
        }
    }
}
