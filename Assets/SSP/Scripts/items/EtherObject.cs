using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class EtherObject : MonoBehaviour
{

    public float etherValue;

    [SerializeField] private float originEtherSize;
    [SerializeField] private float triggerSize = 0.5f;
    [SerializeField] private float floatHeight;
    [SerializeField] private float trackingSpeed;
    private SphereCollider trigger;

    private RaycastHit fallHit;
    private int layerMask = 1 << LayerMap.Stage;

    private RaycastHit playerHit;
    private GameObject target;

    private void Start()
    {
        var rigid = GetComponent<Rigidbody>();

        //地面よりある程度高い位置で重力をきる処理
        this.UpdateAsObservable()
            .Where(_ => Physics.Raycast(transform.position, Vector3.down, out fallHit, floatHeight * 10, layerMask))
            .Where(_ => Vector3.Distance(transform.position, fallHit.point) < floatHeight)
            .Take(1)
            .Subscribe(_ => rigid.useGravity = false);

        this.OnTriggerStayAsObservable()
            .Where(col => col.gameObject.tag == TagMap.Player)
            .Where(col => col.GetComponent<PlayerHealthManager>().IsAlive())
            .Where(col => Physics.Raycast(transform.position, col.transform.position - transform.position, out playerHit, 100))
            .Where(_ => playerHit.collider.gameObject.tag == TagMap.Player)
            .Subscribe(col => target = col.gameObject);
            {
                rigid.AddForce((col.transform.position - transform.position) * trackingSpeed, ForceMode.Force);
            });
    }

    public void Init(float value)
    {
        etherValue = value;
        transform.localScale = Vector3.one * originEtherSize * value;
        if (transform.localScale.x < 1)
            GetTrigger().radius = triggerSize / transform.localScale.x;
    }

    private SphereCollider GetTrigger()
    {
        if (trigger != null) return trigger;
        trigger = GetComponents<SphereCollider>().Where(v => v.isTrigger).First();
        return trigger;
    }
}
