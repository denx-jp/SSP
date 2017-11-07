using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UniRx;
using UnityEngine.Networking;

public class EtherPopper : NetworkBehaviour
{
    [SerializeField] private GameObject ether;
    [SerializeField] private float popInterval;
    [SerializeField] private float popDuration;
    [SerializeField] private int initEtherValue;
    [SerializeField] private List<Transform> popPoints = new List<Transform>();
    private Transform popPoint;

    public void Init()
    {
        if (isServer)
        {
            float timeCounter = 0.0f;
            Observable.Interval(TimeSpan.FromSeconds(popInterval)).TakeWhile(_ => timeCounter <= popDuration).Subscribe(_ =>
            {
                timeCounter += popInterval;
                CmdSpawnEtherObject();
            }).AddTo(this);
        }
    }

    [Command]
    void CmdSpawnEtherObject()
    {
        popPoint = popPoints[UnityEngine.Random.Range(0, popPoints.Count)];
        var etherObject = Instantiate(ether, popPoint.position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(etherObject, NetworkServer.connections[0]);//NetworkPlayerに紐づいていないためConnectionToClientではなくHostの権限でSpawn
        var etherInfo = etherObject.GetComponent<EtherObject>();
        etherInfo.CmdSetEtherValue(initEtherValue);
    }

#if UNITY_EDITOR
    [ContextMenu("Set Pop Points")]
    private void SetPopPoints()
    {
        var childTransforms = this.GetComponentsInChildren<Transform>();
        foreach (var childTransform in childTransforms)
        {
            if (childTransform.gameObject.name == "PopPoint")
                popPoints.Add(childTransform);
        }
    }
#endif
}
