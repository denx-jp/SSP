using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class ShortRangeWeapon : NetworkBehaviour, IWeapon
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] WeaponModel model;
    [SerializeField] float hitDetectionTimeOffset;//攻撃開始から当たり判定が発生するまでの時間
    [SerializeField] float hitDetectionDuration;//当たり判定が発生する時間の長さ

    private bool isAttacking;
    private bool detectable;
    private PlayerModel playerModel;
    private PlayerAnimationController animationController;

    // 装備中でなくなった時の処理
    private void OnDisable()
    {
        isAttacking = false;
        detectable = false;
        if (playerModel != null)
            playerModel.MoveMode = MoveMode.normal;
    }

    public void Init(PlayerManager playerManager)
    {
        model.ownerPlayerModel = playerManager.playerModel;
        animationController = playerManager.playerAnimationController;
        playerModel = playerManager.playerModel;

        isAttacking = false;
        detectable = false;

        //ダメージ判定は攻撃したプレイヤーのクライントでのみ行う
        if (model.ownerPlayerModel.isLocalPlayerCharacter)
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
        if (!isAttacking)
            CmdAttack();
    }

    public void NormalAttackLong(bool active)
    {

    }

    public void SwitchScope()
    {
        playerModel.MoveMode = playerModel.MoveMode == MoveMode.normal ? MoveMode.battle : MoveMode.normal;
    }

    public void LongPressScope(bool active)
    {
        playerModel.MoveMode = active ? MoveMode.battle : MoveMode.normal;
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
        isAttacking = true;
        animationController.Attack();
        yield return new WaitForSeconds(hitDetectionTimeOffset);
        detectable = true;
        gameObject.layer = LayerMap.Attack;
        audioSource.Play();
        yield return new WaitForSeconds(hitDetectionDuration);
        detectable = false;
        gameObject.layer = LayerMap.Default;
        isAttacking = false;
    }

    [Command]
    void CmdSetDamage(GameObject go, Damage dmg)
    {
        var damageable = go.GetComponent<IDamageable>();
        damageable.SetDamage(dmg);
    }
}
