﻿using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
            Instantiate(ether, popPoint.position, Quaternion.identity);
        }).AddTo(this);

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
