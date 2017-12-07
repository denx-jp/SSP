using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerHealthManager : NetworkBehaviour, IDamageable
{
    [SerializeField] private float deathHeightThreshHold = -30f;

    private PlayerModel playerModel;
    private Subject<bool> deathStream = new Subject<bool>();
    public int recentAttackerId { get; private set; }
    private bool isDeath = false;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        deathStream.OnNext(false);

        playerModel.Health
            .Where(v => v <= 0.0f)
            .Subscribe(_ =>
                {
                    if (!isDeath)
                    {
                        deathStream.OnNext(true);
                        isDeath = true;
                    }
                }
            );

        // 範囲外に落下したときは死亡させる。本当はHealthManagerに持たせない方がいいのかもしれない。
        this.ObserveEveryValueChanged(_ => transform.position.y)
            .Where(v => !isDeath && v < deathHeightThreshHold)
            .Subscribe(_ =>
            {
                RpcSyncRecentAttackerID(playerModel.Id);
                playerModel.syncHealth -= playerModel.syncHealth;
            });
    }

    public void SetDamage(Damage damage)
    {
        //フレンドリーファイアはできないように
        if (damage.AttackerTeamId == playerModel.teamId) return;

        if (playerModel.Health.Value > 0.0f && damage.amount > 0.0f)
        {
            RpcSyncRecentAttackerID(damage.AttackerPlayerId);
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
        isDeath = false;
    }

    [ClientRpc]
    void RpcSyncRecentAttackerID(int attackerID)
    {
        recentAttackerId = attackerID;
    }
}
