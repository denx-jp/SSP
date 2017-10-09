using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class ShortRangeWeapon : NetworkBehaviour, IAttackable
{
    [SerializeField] public float damageAmount;//攻撃のダメージ量
    [SerializeField] float hitDetectionTimeOffset;//攻撃開始から当たり判定が発生するまでの時間
    [SerializeField] float hitDetectionDuration;//当たり判定が発生する時間の長さ
    private bool isAttackStarted;
    private bool detectable;
    [SyncVar] private int playerId, teamId;
    private Animator animator;

    private void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Where(_ => detectable)
            .Where(col => col.gameObject.layer != LayerMap.Invincible)
            .Where(col => !col.isTrigger)
            .Subscribe(col =>
            {
                var damageable = col.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    var damage = new Damage(damageAmount, playerId, teamId);
                    CmdSetDamage(col.gameObject, damage);
                }
            });

    }

    public void Init(PlayerModel playerModel)
    {
        isAttackStarted = false;
        playerId = playerModel.playerId;
        teamId = playerModel.teamId;
        animator = playerModel.gameObject.GetComponent<Animator>();
    }

    public void NormalAttack()
    {
        CmdAttack();
    }

    [Command]
    private void CmdAttack()
    {
        RpcAttack();
    }

    [ClientRpc]
    private void RpcAttack()
    {
        animator.SetTrigger("Attack");
        if (isAttackStarted) StopCoroutine(Attacking());
        StartCoroutine(Attacking());
    }

    IEnumerator Attacking()
    {
        isAttackStarted = true;
        yield return new WaitForSeconds(hitDetectionTimeOffset);
        detectable = true;
        gameObject.layer = LayerMap.Attack;
        yield return new WaitForSeconds(hitDetectionDuration);
        detectable = false;
        gameObject.layer = LayerMap.Default;
        isAttackStarted = false;
    }

    [Command]
    void CmdSetDamage(GameObject go, Damage dmg)
    {
        var damageable = go.GetComponent<IDamageable>();
        damageable.SetDamage(dmg);
    }
}
