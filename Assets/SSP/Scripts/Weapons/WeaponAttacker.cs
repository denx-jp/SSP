using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System.Linq;

public class WeaponAttacker : MonoBehaviour
{
    [SerializeField] public float damageAmount;//攻撃のダメージ量
    [SerializeField] float hitDetectionTimeOffset;//攻撃開始から当たり判定が発生するまでの時間
    [SerializeField] float hitDetectionDuration;//当たり判定が発生する時間の長さ
    bool isAttackStarted;
    bool detectable;
    int parentPlayerId;

    void Start()
    {
        parentPlayerId = this.transform.GetComponentInParent<PlayerModel>().playerId;
        this.Init();
    }

    void Init()
    {
        isAttackStarted = false;
    }

    public void NormalAttack(Animator animator)
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
        var hm = col.gameObject.GetComponent<IDamageable>();
        if (hm != null)
        {
            var damage = new Damage(damageAmount, parentPlayerId);
            CmdSetDamage(hm, damage);
        }
    }

    //今後ネットワークにするためCmd
    void CmdSetDamage(IDamageable hm, Damage dmg)
    {
        hm.SetDamage(dmg);
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
