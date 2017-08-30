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

    //float detectionTimer;

    void Start()
    {
        this.Init();
    }

    void Init()
    {
        isAttackStarted = false;
        //detectionTimer = 0.0f;
    }

    public void NormalAttack(Animator animator)
    {
        animator.SetTrigger("Attack");
        //detectionTimer = 0.0f;
        if (isAttackStarted) StopCoroutine(Attacking());
        StartCoroutine(Attacking());
    }

    void OnTriggerEnter(Collider col)
    {
        //if (!GetCanDetectObject) return;
        if (!detectable) return;
        if (col.gameObject.layer == LayerMap.Invincible) return;
        if (col.isTrigger) return; //Colliderのみと衝突を判定する
        var hm = col.gameObject.GetComponent<HealthModel>();
        if(hm != null)
        {
            CmdSetDamage(hm, this.damageAmount);
        }
    }

    //今後ネットワークにするためCmd
    void CmdSetDamage(HealthModel hm,float dmgAmount)
    {
        hm.SetDamage(dmgAmount);
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

    //bool GetCanDetectObject()
    //{
    //    if (!isAttackStarted) return false;
    //    if (detectionTimer < hitDetectionTimeOffset) return false;
    //    if (detectionTimer < hitDetectionDuration) return true;
    //    return false;
    //}

    //void Update()
    //{
    //    if (isAttackStarted)
    //    {
    //        detectionTimer += Time.deltaTime;
    //    }
    //    if(detectionTimer > (hitDetectionTimeOffset + hitDetectionDuration))
    //    {
    //        Init();
    //    }

    //    if (GetCanDetectObject)
    //    {
    //        SetLayer(LayerMap.Attack);
    //    }
    //    else
    //    {
    //        SetLayer(LayerMap.Default);
    //    }
    //}


}
