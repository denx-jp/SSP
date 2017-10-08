using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LongRangeWeapon : MonoBehaviour, IAttackable
{
    [SerializeField] private float coolTime, bulletSpeed;
    [SerializeField] GameObject bulletPrefab, muzzle;
    [SerializeField] private float bulletDamageAmount, bulletDeathTime;
    private BulletModel bulletModel;
    private bool canAttack = true;
    private int playerId, teamId;
    private RaycastHit hit;
    private int layerMask = ~(1 << LayerMap.LocalPlayer);
    private float time = 0;

    public void Init(PlayerModel playerModel)
    {
        playerId = playerModel.playerId;
        teamId = playerModel.teamId;

        this.UpdateAsObservable()
            .Where(_ => this.gameObject.activeSelf)
            .Subscribe(_ =>
            {
                time += Time.deltaTime;
                if (time >= coolTime)
                    canAttack = true;
            });
    }

    public void NormalAttack(Animator animator)
    {
        if (canAttack)
        {
            Shoot();
            time = 0;
            canAttack = false;
        }
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation);
        //レイが衝突すればその点へ飛ばし、衝突しなければそれっぽいところへ飛ばす。
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, layerMask))
        {
            bullet.transform.LookAt(hit.point);
        }
        else
        {
            bullet.transform.rotation = Camera.main.transform.rotation;
        }

        bullet.GetComponent<BulletModel>().SetProperties(playerId, teamId, bulletDamageAmount, bulletDeathTime);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletSpeed);
    }
}
