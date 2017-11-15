﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(HitscanModel))]
public class SniperRifle : LongRangeWeapon, IWeapon
{
    [SerializeField] private GameObject bullet;

    private HitscanModel hitscanModel;
    private RaycastHit damageHit;
    private RaycastHit shootHit;
    protected int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;


    private void Start()
    {
        hitscanModel = (HitscanModel)model;
    }

    protected override void Shoot()
    {
        // hitがないときはreturn
        if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out damageHit, hitscanModel.RayDistance, layerMask))
        {
            CmdShoot();
            return;
        }

        var hitObj = damageHit.collider.gameObject;
        var damageable = hitObj.GetComponent<IDamageable>();

        if (damageable != null)
        {
            var damage = new Damage(model.damageAmount, model.playerId, model.teamId);
            CmdDamageShoot(hitObj, damage);
        }
        else
        {
            CmdShoot();
        }
    }

    [Command]
    void CmdShoot()
    {
        RpcShoot();
    }

    [Command]
    void CmdDamageShoot(GameObject go, Damage dmg)
    {
        RpcShoot();

        var damageable = go.GetComponent<IDamageable>();
        damageable.SetDamage(dmg);
    }

    [ClientRpc]
    private void RpcShoot()
    {
        var bulletInstance = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out shootHit, 1000, layerMask))
            bulletInstance.transform.LookAt(damageHit.point);
        else
            bulletInstance.transform.rotation = cameraTransform.rotation;

        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * hitscanModel.bulletVelocity;
        Destroy(bulletInstance, hitscanModel.bulletDeathTime);

        audioSource.Play();
    }
}
