using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class HandGun : NetworkBehaviour, IWeapon
{
    [SerializeField] LongRangeWeaponModel model;
    [SerializeField] GameObject muzzle;
    private bool canAttack = true;
    private bool autoShoot = false;
    private float shootTime = 0;
    private Transform cameraTransform;
    private RaycastHit hit;
    private int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;
    private PlayerModel pm;
    private bool isScoped = false;

    public void Init(PlayerModel playerModel)
    {
        model.playerId = playerModel.playerId;
        model.teamId = playerModel.teamId;
        model.isOwnerLocalPlayer = playerModel.isLocalPlayerCharacter;
        cameraTransform = Camera.main.transform;
        pm = playerModel;

        this.FixedUpdateAsObservable()
            .Where(_ => this.gameObject.activeSelf)
            .Where(_ => !canAttack)
            .Subscribe(_ =>
            {
                if (Time.time - shootTime >= model.coolTime)
                    canAttack = true;
            });

        this.ObserveEveryValueChanged(_ => canAttack)
            .Where(v => v)
            .Where(_ => autoShoot)
            .Subscribe(_ => NormalAttack());
    }

    public void NormalAttack()
    {
        if (canAttack && isScoped)
        {
            canAttack = false;
            shootTime = Time.time;
            CmdShoot(cameraTransform.position, cameraTransform.forward, cameraTransform.rotation);
        }
    }

    public void NormalAttackLong(bool active)
    {
        NormalAttack();
        autoShoot = active;
    }

    public void LongPressScope(bool active)
    {
        isScoped = active;
    }

    [Command]
    private void CmdShoot(Vector3 castPosition, Vector3 castDirection, Quaternion uncastableDirection)
    {
        var bulletInstance = Instantiate(model.bullet, muzzle.transform.position, muzzle.transform.rotation);
        if (Physics.Raycast(castPosition, castDirection, out hit, 1000, layerMask))
            bulletInstance.transform.LookAt(hit.point);
        else
            bulletInstance.transform.rotation = uncastableDirection;

        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * model.bulletVelocity;
        NetworkServer.SpawnWithClientAuthority(bulletInstance.gameObject, pm.connectionToClient);
        RpcShoot(bulletInstance);
    }

    [ClientRpc]
    private void RpcShoot(GameObject bulletInstance)
    {
        bulletInstance.GetComponent<BulletManager>().Init(model);
    }
}
