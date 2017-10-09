using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class ShortRangeWeapon : NetworkBehaviour, IAttackable
{
    [SerializeField] WeaponModel model;
    [SerializeField] float hitDetectionTimeOffset;//攻撃開始から当たり判定が発生するまでの時間
    [SerializeField] float hitDetectionDuration;//当たり判定が発生する時間の長さ
    private bool detectable;
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
        model.playerId = playerModel.playerId;
        model.teamId = playerModel.teamId;
        model.isOwnerLocalPlayer = playerModel.isLocalPlayerCharacter;
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
        StartCoroutine(Attacking());
    }

    IEnumerator Attacking()
    {
        yield return new WaitForSeconds(hitDetectionTimeOffset);
        detectable = true;
        gameObject.layer = LayerMap.Attack;
        yield return new WaitForSeconds(hitDetectionDuration);
        detectable = false;
        gameObject.layer = LayerMap.Default;
    }

    [Command]
    void CmdSetDamage(GameObject go, Damage dmg)
    {
        var damageable = go.GetComponent<IDamageable>();
        damageable.SetDamage(dmg);
    }
}
