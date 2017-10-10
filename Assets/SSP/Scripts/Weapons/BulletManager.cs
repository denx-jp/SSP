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
        if (isServer)
            Destroy(this.gameObject, model.deathTime);

        if (model.isShooterLocalPlayer)
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
                        NetworkServer.Destroy(this.gameObject);
                    }
                })
                .AddTo(this.gameObject);
        }
    }

    [Command]
    void CmdSetDamage(GameObject go, Damage dmg)
    {
        var damageable = go.GetComponent<IDamageable>();
        damageable.SetDamage(dmg);
    }
}
