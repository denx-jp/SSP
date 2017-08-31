using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerHealthManager : MonoBehaviour, IHealth
{
    [SerializeField] private float initialHealth;
    private PlayerModel playerModel;

    private Subject<bool> deathStream;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        playerModel.Health.Value = initialHealth;
        deathStream = new Subject<bool>();
        deathStream.OnNext(false);

        playerModel.Health.Where(v => v <= 0.0f).Subscribe(_ => deathStream.OnNext(true));
    }

    public bool IsAlive()
    {
        return playerModel.Health.Value > 0.0f;
    }

    public void SetDamage(Damage damage)
    {
        if (playerModel.Health.Value > 0.0f && damage.amount > 0.0f)
        {
            playerModel.Health.Value -= damage.amount;
        }
    }

    public float GetHealth()
    {
        return playerModel.Health.Value;
    }

    public Subject<bool> GetDeathStream()
    {
        return deathStream;
    }
}
