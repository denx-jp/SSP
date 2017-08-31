using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerHealthManager : MonoBehaviour, IHealth
{
    [SerializeField] private float initialHealth;
    private PlayerModel palyerModel;

    private void Start()
    {
        palyerModel = GetComponent<PlayerModel>();
        palyerModel.Health.Value = initialHealth;
    }

    public bool IsAlive()
    {
        return palyerModel.Health.Value > 0.0f;
    }

    public void SetDamage(Damage damage)
    {
        if (palyerModel.Health.Value > 0.0f && damage.amount > 0.0f)
        {
            palyerModel.Health.Value -= damage.amount;
        }
    }

    public float GetHealth()
    {
        return palyerModel.Health.Value;
    }
}
