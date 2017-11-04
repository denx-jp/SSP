﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerHealthManager : NetworkBehaviour, IDamageable
{
    private PlayerModel playerModel;
    private Subject<bool> deathStream;

    public int recentAttackerId { get; private set; }

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        deathStream = new Subject<bool>();
        deathStream.OnNext(false);

        playerModel.Health
            .Where(v => v <= 0.0f)
            .Subscribe(_ => deathStream.OnNext(true));
    }

    public void SetDamage(Damage damage)
    {
        //フレンドリーファイアはできないように
        if (damage.teamId == playerModel.teamId) return;

        if (playerModel.Health.Value > 0.0f && damage.amount > 0.0f)
        {
            recentAttackerId = damage.playerId;
            playerModel.syncHealth -= damage.amount;
        }
    }

    public Subject<bool> GetDeathStream()
    {
        return deathStream;
    }

    public void Revive()
    {
        playerModel.Init();
        deathStream.OnNext(false);
    }
}
