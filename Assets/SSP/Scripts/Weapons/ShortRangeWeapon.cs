using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class ShortRangeWeapon : NetworkBehaviour, IWeapon
{
    [SerializeField] WeaponModel model;
    [SerializeField] float hitDetectionTimeOffset;//攻撃開始から当たり判定が発生するまでの時間
    [SerializeField] float hitDetectionDuration;//当たり判定が発生する時間の長さ
    private bool detectable;
    private PlayerAnimationController animationController;

    public void Init(PlayerManager playerManager)
    {
        model.playerId = playerManager.playerModel.playerId;
        model.teamId = playerManager.playerModel.teamId;
        model.isOwnerLocalPlayer = playerManager.playerModel.isLocalPlayerCharacter;
        animationController = playerManager.gameObject.GetComponent<PlayerAnimationController>();

        //ダメージ判定は攻撃したプレイヤーのクライントでのみ行う
        if (model.isOwnerLocalPlayer)
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
                        var damage = model.GetDamage();
                        CmdSetDamage(col.gameObject, damage);
                        detectable = false;     //リモートクライアントで何故か当たり判定が2回でるのでフラグで制御
                    }
                });
        }
    }

    public void NormalAttack()
    {
        CmdAttack();
    }

    public void NormalAttackLong(bool active)
    {

    }

    public void LongPressScope(bool active)
    {

    }

    [Command]
    private void CmdAttack()
    {
        RpcAttack();
    }

    [ClientRpc]
    private void RpcAttack()
    {
        StartCoroutine(Attacking());
    }

    IEnumerator Attacking()
    {
        animationController.Attack();
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
