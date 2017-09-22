using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class BulletManager : MonoBehaviour
{
    public void SetBulletDeath(GameObject g, GameObject parent,float death)
    {
        Destroy(g, death);
        g.OnTriggerEnterAsObservable().Where(x => x.transform.root.gameObject != parent && !x.isTrigger).Subscribe(_ => Destroy(g)).AddTo(g);
    }
}
