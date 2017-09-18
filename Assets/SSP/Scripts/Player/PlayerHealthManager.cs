using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    private PlayerModel playerModel;
    private Subject<bool> deathStream;
    private Subject<KeyValuePair<int,int>> killLogStream;

    private int deathHash = Animator.StringToHash("Death");
    private Animator animator;

    private int recentAttackerId;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        deathStream = new Subject<bool>();
        killLogStream = new Subject<KeyValuePair<int, int>>();
        deathStream.OnNext(false);

        playerModel.Health
            .Where(v => v <= 0.0f)
            .Subscribe(_ => deathStream.OnNext(true));

        animator = GetComponent<Animator>();
        this.deathStream
             .Subscribe(isdeath =>
             {
                 var myId = this.transform.GetComponentInParent<PlayerModel>().playerId;
                 killLogStream.OnNext(new KeyValuePair<int, int>(recentAttackerId, myId));
                 animator.SetBool(deathHash, isdeath);
             });
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

    public Subject<KeyValuePair<int,int>> GetKillLogStream(){
        return killLogStream;
    }
}
