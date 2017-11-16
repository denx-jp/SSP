using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class LongRangeWeapon : NetworkBehaviour, IWeapon
{
    [SerializeField] protected LongRangeWeaponModel model;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected Vector3 gunHoldOffset;
    [SerializeField] protected Vector3 leftHandOffset;
    [SerializeField] protected Vector3 socpeCameraOffset;

    protected Transform cameraTransform;
    protected float shootTime = 0;
    protected bool canShoot = true;
    protected bool autoShoot = false;
    protected bool isScoped = false;

    protected PlayerModel playerModel;
    protected PlayerIKPoser ikPoser;
    protected PlayerCameraController pcc;
    protected AudioSource audioSource;

    private void OnEnable()
    {
        if (playerModel != null && playerModel.MoveMode == MoveMode.battle)
            isScoped = true;
        
        if (localPlayerAuthority && ikPoser != null)
        {
            ikPoser.SetAimTransform(muzzle);
            ikPoser.CmdSetHandOffset(gunHoldOffset, leftHandOffset);
        }
    }

    // 装備中でなくなった時の処理
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

        cameraTransform = Camera.main.gameObject.transform;

        pcc = playerManager.playerCameraController;
        audioSource = GetComponent<AudioSource>();

        this.FixedUpdateAsObservable()
            .Where(_ => this.gameObject.activeSelf)
            .Where(_ => !canShoot)
            .Where(_ => Time.time - shootTime >= model.coolTime)
            .Subscribe(_ => canShoot = true);

        this.ObserveEveryValueChanged(_ => canShoot)
            .Where(v => v)
            .Where(_ => autoShoot)
            .Subscribe(_ => NormalAttack());

        this.UpdateAsObservable()
            .Where(_ => playerModel.MoveMode == MoveMode.battle)
            .Where(_ => hasAuthority)
            .Subscribe(_ =>
            {
                ikPoser.CmdSetTarget(cameraTransform.position + (cameraTransform.forward * 10));
            });
    }

    #region IWeaponメソッド
    public void NormalAttack()
    {
        if (canShoot && isScoped)
        {
            canShoot = false;
            shootTime = Time.time;
            Shoot();
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
            pcc.FitRotate();
            pcc.SetScopeOffset(socpeCameraOffset);
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
        if (isScoped && active) return;   // スコープ中はaim不可

        isScoped = active;
        if (isScoped)
        {
            pcc.FitRotate();
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

    protected virtual void Shoot()
    {
        // 各武器でoverrideする
    }
}
