using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class HandGun : NetworkBehaviour, IWeapon
{
    [SerializeField] private LongRangeWeaponModel model;
    [SerializeField] private GameObject muzzle;
    [SerializeField] private GameObject scopeCamera;
    private GameObject mainCamera;
    private Transform cameraTransform;

    private bool canAttack = true;
    private bool autoShoot = false;
    private bool isScoped = false;
    private float shootTime = 0;

    private RaycastHit hit;
    private int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;

    private PlayerModel pm;
    private PlayerCameraController pcc;
    private AudioSource audioSource;

    public void Init(PlayerModel playerModel)
    {
        pm = playerModel;

        model.playerId = playerModel.Id;
        model.teamId = playerModel.teamId;
        model.isOwnerLocalPlayer = playerModel.isLocalPlayerCharacter;

        scopeCamera.gameObject.SetActive(false);
        mainCamera = Camera.main.gameObject;
        cameraTransform = mainCamera.transform;
        pcc = pm.gameObject.GetComponent<PlayerCameraController>();
        audioSource = this.gameObject.GetComponent<AudioSource>();

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

    #region IWeaponメソッド
    public void NormalAttack()
    {
        if (canAttack && isScoped)
        {
            canAttack = false;
            shootTime = Time.time;
            CmdShoot(cameraTransform.position, cameraTransform.forward, cameraTransform.rotation);
        }
    }

    public void SwitchScope()
    {
        var toScope = !scopeCamera.activeSelf;
        pcc.SwitchCamera(toScope, scopeCamera);
        // Rayを飛ばすカメラを切り替える
        cameraTransform = toScope ? scopeCamera.transform : mainCamera.transform;
        isScoped = scopeCamera.activeSelf;
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
    #endregion

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
        audioSource.Play();
    }
}
