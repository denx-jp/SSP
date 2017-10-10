using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class LongRangeWeapon : NetworkBehaviour, IAttackable
{
    [SerializeField] LongRangeWeaponModel model;
    [SerializeField] GameObject muzzle;
    private bool canAttack = true;
    private float reloadTime = 0;
    private Transform cameraTransform;
    private RaycastHit hit;
    private int layerMask = ~(1 << LayerMap.LocalPlayer);

    public void Init(PlayerModel playerModel)
    {
        model.playerId = playerModel.playerId;
        model.teamId = playerModel.teamId;
        model.isOwnerLocalPlayer = playerModel.isLocalPlayerCharacter;
        cameraTransform = Camera.main.transform;

        this.UpdateAsObservable()
            .Where(_ => this.gameObject.activeSelf)
            .Subscribe(_ =>
            {
                reloadTime += Time.deltaTime;
                if (reloadTime >= model.coolTime)
                    canAttack = true;
            });
    }

    public void NormalAttack()
    {
        if (canAttack)
        {
            CmdShoot(cameraTransform.position, cameraTransform.forward, cameraTransform.rotation);
            reloadTime = 0;
            canAttack = false;
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
        bulletInstance.GetComponent<BulletModel>().SetProperties(model);
        NetworkServer.Spawn(bulletInstance.gameObject);
    }
}
