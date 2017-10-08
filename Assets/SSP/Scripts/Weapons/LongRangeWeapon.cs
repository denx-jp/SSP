using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class LongRangeWeapon : NetworkBehaviour, IAttackable
{
    [SerializeField] private float coolTime, bulletSpeed = 1000;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Rigidbody bulletRigid;
    [SerializeField] GameObject muzzle;
    [SerializeField] private float bulletDamageAmount, bulletDeathTime = 5;
    private BulletModel bulletModel;
    private bool canAttack = true;
    [SyncVar] private int playerId, teamId;
    private RaycastHit hit;
    private int layerMask = ~(1 << LayerMap.LocalPlayer);
    private float time = 0;
    private Transform cameraTransform;

    public void Init(PlayerModel playerModel)
    {
        playerId = playerModel.playerId;
        teamId = playerModel.teamId;
        cameraTransform = Camera.main.transform;

        this.UpdateAsObservable()
            .Where(_ => this.gameObject.activeSelf)
            .Subscribe(_ =>
            {
                time += Time.deltaTime;
                if (time >= coolTime)
                    canAttack = true;
            });
    }

    public void NormalAttack()
    {
        if (canAttack)
        {
            CmdShoot(cameraTransform.position, cameraTransform.forward, cameraTransform.rotation);
            time = 0;
            canAttack = false;
        }
    }

    [Command]
    private void CmdShoot(Vector3 castPosition, Vector3 castDirection, Quaternion uncastableDirection)
    {
        Rigidbody bulletInstance = Instantiate(bulletRigid, muzzle.transform.position, muzzle.transform.rotation) as Rigidbody;
        if (Physics.Raycast(castPosition, castDirection, out hit, 1000, layerMask))
            bulletInstance.transform.LookAt(hit.point);
        else
            bulletInstance.transform.rotation = uncastableDirection;

        bulletInstance.velocity = bulletInstance.transform.forward * bulletSpeed;
        bulletInstance.GetComponent<BulletModel>().SetProperties(playerId, teamId, bulletDamageAmount, bulletDeathTime);
        NetworkServer.Spawn(bulletInstance.gameObject);
    }
}
