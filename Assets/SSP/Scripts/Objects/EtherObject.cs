using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class EtherObject : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField, SyncVar] public float etherValue;
    [SerializeField] private float floatHeight;
    [SerializeField] private float trackingSpeed;
    [SerializeField] private float popEtherInitValue = 50;
    [HideInInspector] public GameObject target;

    private SphereCollider trigger;
    private RaycastHit fallHit;
    private RaycastHit absorbHit;
    private int absorbLayerMask = ~(LayerMap.EtherObjectMask);

    public void Init(float value)
    {
        etherValue = value;
    }

    private void Start()
    {
        //エーテル自動popのため
        if (etherValue == 0)
            Init(popEtherInitValue);

        //地面よりある程度高い位置で重力をきる処理
        this.UpdateAsObservable()
            .Where(_ => Physics.Raycast(transform.position, Vector3.down, out fallHit, floatHeight * 10, LayerMap.StageMask))
            .Where(_ => Vector3.Distance(transform.position, fallHit.point) < floatHeight)
            .Take(1)
            .Subscribe(_ => rigid.useGravity = false);

        #region エーテル吸収処理
        //targetを追従
        this.FixedUpdateAsObservable()
            .Where(_ => target != null)
            .Select(v => target.transform.position + Vector3.up)    // target.transform.positionだと地面をはいずって追いかけてみっともない感じがするのでちょっと上を追従する
            .Subscribe(v => rigid.AddForce((v - transform.position) * trackingSpeed, ForceMode.Force));

        //targetに衝突時に消滅・吸収
        this.OnCollisionEnterAsObservable()
            .TakeWhile(_ => hasAuthority)
            .Where(col => col.gameObject == target)
            .Subscribe(_ => CmdEther(target));
        #endregion
    }

    public void SetAndSyncTarget(GameObject go)
    {
        if (isServer)
            RpcTarget(go);
    }

    [ClientRpc]
    void RpcTarget(GameObject go)
    {
        target = go;
    }

    [Command]
    void CmdEther(GameObject go)
    {
        go.GetComponent<IEtherAcquirer>().AcquireEther(etherValue);
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    public void CmdSetEtherValue(int etherValue)
    {
        Init(etherValue);
    }
}

