using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class LongRangeWeapon : NetworkBehaviour, IAttackable
{
    [SerializeField] LongRangeWeaponModel model;
    [SerializeField] GameObject muzzle;
    private bool canAttack = true;
    private float shootTime = 0;
    private Transform cameraTransform;
    private RaycastHit hit;
    private int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;
    private PlayerModel pm;
    private PlayerInputManager pim;

    private void Start()
    {
        pim = transform.root.GetComponent<PlayerInputManager>();
        var scoped = this.UpdateAsObservable().SkipUntil(pim.ScopeButtonLong.Where(x => x)).TakeUntil(pim.ScopeButtonLong.Where(x => !x)).RepeatUntilDestroy(gameObject);
        //単発
        pim.AttackButtonDown.SkipUntil(scoped).Where(down => down).Subscribe(_ => Debug.Log("single"));
        //フルオート
    }

    public void Init(PlayerModel playerModel)
    {
        model.playerId = playerModel.playerId;
        model.teamId = playerModel.teamId;
        model.isOwnerLocalPlayer = playerModel.isLocalPlayerCharacter;
        cameraTransform = Camera.main.transform;
        pm = playerModel;

        pim = transform.root.GetComponent<PlayerInputManager>();
        //単発
        //フルオート

        Debug.Log("init");
        this.FixedUpdateAsObservable()
            .Where(_ => this.gameObject.activeSelf)
            .Where(_ => !canAttack)
            .SkipUntil(pim.AttackButtonLong.Where(t => t))
            .TakeUntil(pim.AttackButtonLong.Where(f => !f))
            .RepeatUntilDestroy(gameObject)
            .Subscribe(_ =>
            {
                Debug.Log("can");
                if (Time.time - shootTime >= model.coolTime)
                    canAttack = true;
            });
    }

    public void NormalAttack()
    {
        if (canAttack)
        {
            Debug.Log("attack");
            shootTime = Time.time;
            canAttack = false;
            CmdShoot(cameraTransform.position, cameraTransform.forward, cameraTransform.rotation);
        }
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
