using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class LongRangeWeapon : NetworkBehaviour, IAttackable
{
    [SerializeField] LongRangeWeaponModel model;
    [SerializeField] GameObject muzzle;
    [HideInInspector] public bool scoped = false;
    private bool canAttack = true;
    private float shootTime = 0;
    private Transform cameraTransform;
    private RaycastHit hit;
    private int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;
    private PlayerModel pm;

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
    }

    public void NormalAttack()
    {
        if (canAttack && scoped)
        {
            shootTime = Time.time;
            canAttack = false;
            CmdShoot(cameraTransform.position, cameraTransform.forward, cameraTransform.rotation);
        }
    }

    public void LongPressScope()
    {
        scoped = scoped ? false : true;
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
