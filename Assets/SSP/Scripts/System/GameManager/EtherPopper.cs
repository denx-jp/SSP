using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Networking;

public class EtherPopper : NetworkBehaviour
{
    [SerializeField] private GameObject ether;
    [SerializeField] private float popInterval;
    [SerializeField] private float popDuration;
    [SerializeField] private int initEtherValue;
    private List<Transform> popPoints = new List<Transform>();

    public void Init()
    {
        if (isServer)
        {
            popPoints = GameObject.FindGameObjectsWithTag(TagMap.PopPoint).Select(v => v.transform).ToList();
            float timeCounter = 0.0f;
            Observable.Interval(System.TimeSpan.FromSeconds(popInterval))
                .TakeWhile(_ => timeCounter <= popDuration)
                .Subscribe(_ =>
                {
                    timeCounter += popInterval;
                    var popPoint = popPoints[Random.Range(0, popPoints.Count)];
                    var etherObject = Instantiate(ether, popPoint.position + Vector3.up * 5, Quaternion.identity);
                    //NetworkPlayerに紐づいていないためConnectionToClientではなくHostの権限でSpawn
                    NetworkServer.SpawnWithClientAuthority(etherObject, NetworkServer.connections[0]);
                    var etherInfo = etherObject.GetComponent<EtherObject>();
                    etherInfo.CmdSetEtherValue(initEtherValue);
                }).AddTo(this);
        }
    }
}
