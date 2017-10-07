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
    private int playerId, teamId;
    private Animator animator;

    public void Init(PlayerModel playerModel)
    {
        isAttackStarted = false;
        playerId = playerModel.playerId;
        teamId = playerModel.teamId;
        animator = playerModel.gameObject.GetComponent<Animator>();
    }

    public void NormalAttack()
    {
        animator.SetTrigger("Attack");
        if (isAttackStarted) StopCoroutine(Attacking());
        StartCoroutine(Attacking());
    }

    void OnTriggerEnter(Collider col)
    {
        if (!detectable) return;
        if (col.gameObject.layer == LayerMap.Invincible) return;
        if (col.isTrigger) return; //Colliderのみと衝突を判定する
        var damageable = col.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            var damage = new Damage(damageAmount, playerId, teamId);
            CmdSetDamage(damageable, damage);
        }
    }

    //今後ネットワークにするためCmd
    void CmdSetDamage(IDamageable damageable, Damage dmg)
    {
        damageable.SetDamage(dmg);
    }

    void SetLayer(int layer)
    {
        this.gameObject.layer = layer;
    }

    void SetDetectable(bool _detectable)
    {
        detectable = _detectable;
    }

    IEnumerator Attacking()
    {
        isAttackStarted = true;
        yield return new WaitForSeconds(hitDetectionTimeOffset);
        SetDetectable(true);
        SetLayer(LayerMap.Attack);
        yield return new WaitForSeconds(hitDetectionDuration);
        SetDetectable(false);
        SetLayer(LayerMap.Default);
        isAttackStarted = false;
    }
}
