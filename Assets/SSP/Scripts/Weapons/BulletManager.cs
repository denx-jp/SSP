using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class BulletManager : NetworkBehaviour
{
    [SerializeField] BulletModel model;

    void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Where(col => !col.isTrigger)
            .Subscribe(col =>
            {
                var playerModel = col.gameObject.GetComponent<PlayerModel>();
                if (playerModel != null && playerModel.teamId != model.shootPlayerTeamId)
                {
                    var damageable = col.gameObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        var damage = new Damage(model.damageAmount, model.shootPlayerId, model.shootPlayerTeamId);
                        CmdSetDamage(col.gameObject, damage);
                    }
                    Destroy(this.gameObject);
                }
            })
            .AddTo(this.gameObject);

        Destroy(this.gameObject, model.deathTime);
    }

    [Command]
    void CmdSetDamage(GameObject go, Damage dmg)
    {
        var damageable = go.GetComponent<IDamageable>();
        damageable.SetDamage(dmg);
    }
}
