using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    private PlayerModel playerModel;
    private Subject<bool> deathStream;

    private int deathHash = Animator.StringToHash("Death");
    private Animator animator;

    private int recentAttackerId;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        deathStream = new Subject<bool>();
        deathStream.OnNext(false);

        playerModel.Health
            .Where(v => v <= 0.0f)
            .Subscribe(_ => deathStream.OnNext(true));

        animator = GetComponent<Animator>();
        this.deathStream
             .Subscribe(isdeath =>
             {
                 var myId = this.transform.GetComponentInParent<PlayerModel>().playerId;
                 Debug.Log(string.Format("Player {0} killed Player {1}",recentAttackerId, myId)); //デバッグ用に実装（のちのちUI作ったときに差し替え）
                 animator.SetBool(deathHash, isdeath);
             });
    }

    public bool IsAlive()
    {
        return playerModel.Health.Value > 0.0f;
    }

    public void SetDamage(Damage damage)
    {
        if (playerModel.Health.Value > 0.0f && damage.amount > 0.0f)
        {
            recentAttackerId = damage.id;
            playerModel.Health.Value -= damage.amount;
        }
    }

    public Subject<bool> GetDeathStream()
    {
        return deathStream;
    }
}
