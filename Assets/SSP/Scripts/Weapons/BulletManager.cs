using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class BulletManager : MonoBehaviour
{
    [SerializeField] BulletModel model;

    void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Where(col => col.gameObject.layer != LayerMap.LocalPlayer)
            .Subscribe(col =>
            {
                var damageable = col.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    var damage = new Damage(model.damageAmount, model.shootPlayerId);
                    CmdSetDamage(damageable, damage);
                }
                Destroy(this.gameObject);
            })
            .AddTo(this.gameObject);

        Observable.Timer(TimeSpan.FromSeconds(model.deathTime))
            .Subscribe(_ => Destroy(this.gameObject))
            .AddTo(this.gameObject);
    }

    void CmdSetDamage(IDamageable damageable, Damage dmg)
    {
        damageable.SetDamage(dmg);
    }
}
