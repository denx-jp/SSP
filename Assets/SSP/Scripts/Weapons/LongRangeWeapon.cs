using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LongRangeWeapon : MonoBehaviour
{
    [SerializeField]
    GameObject bullet, muzzle;
    GameObject player;
    PlayerInputManager pim;
    RaycastHit hit;
    float time, coolTime, deathTime, bulletSpeed;

    void Start()
    {
        SetBulletStatus(bullet.GetComponent<BulletModel>());
        player = transform.root.gameObject;
        pim = player.GetComponent<PlayerInputManager>();
        this.UpdateAsObservable().Subscribe(_ => time += Time.deltaTime);
        pim.NormalAttackButtonDown.Where(x => x && time >= coolTime).Subscribe(_ => { time = 0; Shot(); });
    }

    void SetBulletStatus(BulletModel bm)
    {
        coolTime = bm.coolTime;
        deathTime = bm.deathTime;
        bulletSpeed = bm.bulletSpeed;
    }

    void Shot()
    {
        var blt = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);
        //レイが衝突すればその点へ飛ばし、衝突しなければそれっぽいところへ飛ばす。
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit) && hit.transform.gameObject != player)
        {
            blt.transform.LookAt(hit.point);
            Debug.Log("Ray");
        }
        else
        {
            blt.transform.rotation = Camera.main.transform.rotation;
            Debug.Log("Not");
        }
        blt.GetComponent<Rigidbody>().AddForce(blt.transform.forward * bulletSpeed);

        //弾の寿命を他で実装したりObjectPoolするなら以下削除
        Destroy(blt, deathTime);
        blt.OnTriggerEnterAsObservable().Where(x => x.gameObject != player).Do(x => Debug.Log(x)).Subscribe(_ => Destroy(blt)).AddTo(blt);
    }
}
