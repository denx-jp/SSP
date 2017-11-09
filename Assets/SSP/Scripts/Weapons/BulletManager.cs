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

    public void Init(LongRangeWeaponModel lrwm)
    {
        if (isServer)
            Destroy(gameObject, model.deathTime);

        model.SetProperties(lrwm);

        if (model.isShooterLocalPlayer)
        {
            this.OnTriggerEnterAsObservable()
                .Where(col => !col.isTrigger)
                .Subscribe(col =>
                {
                    var damageable = col.gameObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        var damage = new Damage(model.damageAmount, model.shootPlayerId, model.shootPlayerTeamId);
                        CmdSetDamage(col.gameObject, damage);
                    }
                    var playerModel = col.gameObject.GetComponent<PlayerModel>();
                    if (playerModel == null || playerModel.Id != model.shootPlayerId)
                    {
                        GetComponent<NetworkTransform>().enabled = false;
                        CmdDestroy();
                    }
                }).AddTo(this.gameObject);
        }
    }

    [Command]
    void CmdSetDamage(GameObject go, Damage dmg)
    {
        var damageable = go.GetComponent<IDamageable>();
        damageable.SetDamage(dmg);
    }

    [Command]
    void CmdDestroy()
    {
        NetworkServer.Destroy(this.gameObject);
    }
}
