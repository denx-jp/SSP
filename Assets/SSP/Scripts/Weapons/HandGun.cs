using UnityEngine;
using UnityEngine.Networking;

public class HandGun : LongRangeWeapon
{
    [SerializeField] private GameObject bullet;

    private ProjectileModel hgModel;
    private RaycastHit hit;
    private int layerMask = LayerMap.DefaultMask | LayerMap.StageMask;

    private void Start()
    {
        hgModel = (ProjectileModel)model;
    }

    protected override void Shoot()
    {
        CmdShoot(cameraTransform.position, cameraTransform.forward, cameraTransform.rotation);
    }

    [Command]
    private void CmdShoot(Vector3 castPosition, Vector3 castDirection, Quaternion uncastableDirection)
    {
        var bulletInstance = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);
        if (Physics.Raycast(castPosition, castDirection, out hit, 1000, layerMask))
            bulletInstance.transform.LookAt(hit.point);
        else
            bulletInstance.transform.rotation = uncastableDirection;

        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * hgModel.bulletVelocity;
        NetworkServer.SpawnWithClientAuthority(bulletInstance.gameObject, playerModel.connectionToClient);
        RpcShoot(bulletInstance);
    }

    [ClientRpc]
    private void RpcShoot(GameObject bulletInstance)
    {
        bulletInstance.GetComponent<BulletManager>().Init(hgModel);
        audioSource.Play();
    }
}
