using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(HitscanModel))]
public class SniperRifle : LongRangeWeapon, IWeapon
{
    private HitscanModel hitscanModel;
    private RaycastHit hit;
    protected int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;

    private void Start()
    {
        hitscanModel = (HitscanModel)model;
    }

    protected override void Shoot()
    {
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, hitscanModel.RayDistance, layerMask);
        var hitObj = hit.collider.gameObject;
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
        audioSource.Play();
    }
}
