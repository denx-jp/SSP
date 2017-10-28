using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class LifeSupportSystemRespawner : NetworkBehaviour
{
    [SerializeField]private List<Vector3> LSSRespawnPoints;
    [SerializeField]private Transform teamALifeSupportSystemTransform;
    [SerializeField]private Transform teamBLifeSupportSystemTransform;

    private Dictionary<int, bool> availableRespawnPoints;

    [Server]
    void Start()
    {
        availableRespawnPoints = new Dictionary<int, bool>();

        Init();
        if (LSSRespawnPoints.Count >= 2)
        {
            SetPoint(teamALifeSupportSystemTransform);
            SetPoint(teamBLifeSupportSystemTransform);
        }
    }

    void SetPoint(Transform LSSTransform)
    {
        int candidatePoint;
        do
        {
            candidatePoint = UnityEngine.Random.Range(0, LSSRespawnPoints.Count);
        } while (!availableRespawnPoints[candidatePoint]);

        LSSTransform.position = LSSRespawnPoints[candidatePoint];
        availableRespawnPoints[candidatePoint] = false;
    }

    void Init()
    {
        foreach (var point in LSSRespawnPoints.Select((v, i) => new { v, i }))
        {
            availableRespawnPoints.Add(point.i, true);
        }
    }
}