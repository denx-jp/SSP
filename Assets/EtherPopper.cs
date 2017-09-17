using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;

public class EtherPopper : MonoBehaviour
{
    [SerializeField] private GameObject ether;
    [SerializeField] private float popINterval;
    [SerializeField] private List<Transform> popPoints = new List<Transform>();
    private Transform popPoint;

    void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(popINterval)).Subscribe(_ =>
        {
            popPoint = popPoints[UnityEngine.Random.Range(0, popPoints.Count)];
            GameObject popEther = Instantiate(ether, popPoint.position, Quaternion.identity);
        }).AddTo(this);

    }
}
