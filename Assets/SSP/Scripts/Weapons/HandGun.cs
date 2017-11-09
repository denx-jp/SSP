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
    [SerializeField] Transform scope;
    [SerializeField] Vector3 gunHoldOffset;
    [SerializeField] Vector3 leftHandOffset;

    private Transform cameraTransform;

    private float shootTime = 0;
    private bool isReloaded = true;
    private bool autoShoot = false;
    private bool isScoped = false;

    private RaycastHit hit;
    private int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;

    private PlayerModel playerModel;
    private PlayerIKPoser ikPoser;
    private PlayerCameraController pcc;
    private AudioSource audioSource;

    private void OnEnable()
    {
        if (playerModel == null) return;
        if (playerModel.MoveMode == MoveMode.battle) isScoped = true;

        if (ikPoser == null) return;
        ikPoser.gunHoldOffset = gunHoldOffset;
        ikPoser.leftHandOffset = leftHandOffset;

    }

    private void OnDisable()
    {
        isScoped = false;
    }

    public void Init(PlayerManager playerManager)
    {
        model.playerId = playerManager.playerModel.playerId;
        model.teamId = playerManager.playerModel.teamId;
        model.isOwnerLocalPlayer = playerManager.playerModel.isLocalPlayerCharacter;

        playerModel = playerManager.playerModel;
        ikPoser = playerManager.playerIKPoser;
        ikPoser.SetAimTransform(muzzle.transform);

        cameraTransform = Camera.main.gameObject.transform;

        pcc = playerManager.playerCameraController;
        audioSource = GetComponent<AudioSource>();

        this.FixedUpdateAsObservable()
            .Where(_ => this.gameObject.activeSelf)
            .Where(_ => !isReloaded)
            .Where(_ => Time.time - shootTime >= model.coolTime)
            .Subscribe(_ => isReloaded = true);

        this.ObserveEveryValueChanged(_ => isReloaded)
            .Where(v => v)
            .Where(_ => autoShoot)
            .Subscribe(_ => NormalAttack());

        RaycastHit IKHit;
        this.UpdateAsObservable()
            .Where(_ => playerModel.MoveMode == MoveMode.battle)
            .Subscribe(_ =>
            {
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out IKHit, 1000, layerMask))
                {
                    ikPoser.SetTarget(IKHit.point);
                }
                else
                {
                    ikPoser.SetTarget(cameraTransform.position + (cameraTransform.forward * 10));
                }
            });
    }

    #region IWeaponメソッド
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

    public void SwitchScope()
    {
        isScoped = !isScoped;
        if (isScoped)
        {
            pcc.ChangeCameraMode(CameraMode.Scope);
            playerModel.MoveMode = MoveMode.battle;
        }
        else
        {
            pcc.ChangeCameraMode(CameraMode.Normal);
            playerModel.MoveMode = MoveMode.normal;
        }
    }

    public void LongPressScope(bool active)
    {
        isScoped = active;
        if (isScoped)
        {
            pcc.ChangeCameraMode(CameraMode.Battle);
            playerModel.MoveMode = MoveMode.battle;
        }
        else
        {
            pcc.ChangeCameraMode(CameraMode.Normal);
            playerModel.MoveMode = MoveMode.normal;
        }
    }
    #endregion

    #region Shoot
    [Command]
    private void CmdShoot(Vector3 castPosition, Vector3 castDirection, Quaternion uncastableDirection)
    {
        var bulletInstance = Instantiate(model.bullet, muzzle.transform.position, muzzle.transform.rotation);
        if (Physics.Raycast(castPosition, castDirection, out hit, 1000, layerMask))
            bulletInstance.transform.LookAt(hit.point);
        else
            bulletInstance.transform.rotation = uncastableDirection;

        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * model.bulletVelocity;
        NetworkServer.SpawnWithClientAuthority(bulletInstance.gameObject, playerModel.connectionToClient);
        RpcShoot(bulletInstance);
    }

    [ClientRpc]
    private void RpcShoot(GameObject bulletInstance)
    {
        bulletInstance.GetComponent<BulletManager>().Init(model);
        audioSource.Play();
    }
    #endregion
}
