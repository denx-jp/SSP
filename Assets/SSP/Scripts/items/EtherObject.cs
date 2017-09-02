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
    private SphereCollider trigger;

    private RaycastHit hit;
    private int layerMask = 1 << LayerMap.Stage;

    private void Start()
    {
        var rigid = GetComponent<Rigidbody>();

        this.UpdateAsObservable()
            .Where(_ => Physics.Raycast(transform.position, Vector3.down, out hit, floatHeight * 10, layerMask))
            .Where(_ => Vector3.Distance(transform.position, hit.point) < floatHeight)
            .Take(1)
            .Subscribe(_ => rigid.useGravity = false);
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
