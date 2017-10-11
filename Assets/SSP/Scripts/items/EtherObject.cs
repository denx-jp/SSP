using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class EtherObject : NetworkBehaviour
{
    public float etherValue;
    [SerializeField] private float floatHeight;
    [SerializeField] private float trackingSpeed;
    [SerializeField] private float popEtherInitValue = 50;
    [SyncVar] private GameObject target;

    private SphereCollider trigger;
    private RaycastHit fallHit;
    private RaycastHit absorbHit;
    private int absorbLayerMask = ~(LayerMap.EtherObjectMask);


    private void Start()
    {
        var rigid = GetComponent<Rigidbody>();

        //エーテル自動popのため
        if (etherValue == 0)
        {
            Init(popEtherInitValue);
        }

        //地面よりある程度高い位置で重力をきる処理
        this.UpdateAsObservable()
            .Where(_ => Physics.Raycast(transform.position, Vector3.down, out fallHit, floatHeight * 10, LayerMap.StageMask))
            .Where(_ => Vector3.Distance(transform.position, fallHit.point) < floatHeight)
            .Take(1)
            .Subscribe(_ => rigid.useGravity = false);

        #region エーテル吸収処理
        //trigger圏内のPlayerのRayをとばして、障害物がなければtargetに指定
        this.OnTriggerEnterAsObservable()
            .Where(_ => target == null)
            .Where(col => col.gameObject.tag == TagMap.Player)
            .Where(col => col.GetComponent<PlayerModel>().IsAlive())
            .Where(col => Physics.Raycast(transform.position, col.transform.position - transform.position, out absorbHit, 100, absorbLayerMask))
            .Where(_ => absorbHit.collider.gameObject.tag == TagMap.Player)
            .Subscribe(col => target = col.gameObject);

        //targetを追従
        this.UpdateAsObservable()
            .Where(_ => target != null)
            .Subscribe(_ => rigid.AddForce((target.transform.position - transform.position) * trackingSpeed, ForceMode.Force));

        //targetに衝突時に消滅・吸収
        this.OnCollisionEnterAsObservable()
            .Where(col => col.gameObject == target)
            .Subscribe(_ =>
            {
                target.GetComponent<IEtherAcquirer>().AcquireEther(etherValue);
                NetworkServer.Destroy(gameObject);
            });
        #endregion
    }

    public void Init(float value)
    {
        etherValue = value;
    }

}

