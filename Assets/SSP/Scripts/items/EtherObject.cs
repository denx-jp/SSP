using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherObject : MonoBehaviour
{

    public float etherValue;

    [SerializeField] private float originEtherSize;
    [SerializeField] private float floatHeight;
    private Rigidbody rigid;
    private RaycastHit hit;
    private int layerMask = 1 << LayerMap.Stage;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, floatHeight * 10, layerMask))
        {
            var high = Vector3.Distance(transform.position, hit.point);
            if (high < floatHeight)
            {
                rigid.useGravity = false;
            }
        }
    }

    public void SetEther(float value)
    {
        etherValue = value;
        transform.localScale = Vector3.one * originEtherSize * value;
    }
}
