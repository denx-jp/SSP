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
    private bool isReloaded = true;
    private bool autoShoot = false;
    private float shootTime = 0;
    private Transform cameraTransform;
    private RaycastHit hit;
    private int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;
    private PlayerModel pm;
    private bool isScoped = false;

    private void OnEnable()
    {
        if (pm != null && pm.MoveMode == MoveMode.battle)
            isScoped = true;
    }

    private void OnDisable()
    {
        isScoped = false;
    }

    public void Init(PlayerModel playerModel)
    {
        model.playerId = playerModel.playerId;
        model.teamId = playerModel.teamId;
        model.isOwnerLocalPlayer = playerModel.isLocalPlayerCharacter;
        cameraTransform = Camera.main.transform;
        pm = playerModel;

        this.FixedUpdateAsObservable()
            .Where(_ => this.gameObject.activeSelf)
            .Where(_ => !isReloaded)
            .Subscribe(_ =>
            {
                if (Time.time - shootTime >= model.coolTime)
                    isReloaded = true;
            });

        this.ObserveEveryValueChanged(_ => isReloaded)
            .Where(v => v)
            .Where(_ => autoShoot)
            .Subscribe(_ => NormalAttack());
    }

    public void NormalAttack()
    {
        if (isReloaded && isScoped)
        {
            isReloaded = false;
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
