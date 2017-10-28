using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LifeSupportSystemRespawner : MonoBehaviour
{
    [SerializeField]private List<Vector3> LSSRespawnPoints;
    [SerializeField]private Transform teamALifeSupportSystemTransform;
    [SerializeField]private Transform teamBLifeSupportSystemTransform;

    void Start()
    {
        if (LSSRespawnPoints.Count != 0)
        {
            Debug.Log(LSSRespawnPoints.Count);

            int candidatePoint = UnityEngine.Random.Range(0, LSSRespawnPoints.Count);
            Debug.Log(LSSRespawnPoints[candidatePoint]);
            teamALifeSupportSystemTransform.position = LSSRespawnPoints[candidatePoint];

            candidatePoint = UnityEngine.Random.Range(0, LSSRespawnPoints.Count);
            Debug.Log(LSSRespawnPoints[candidatePoint]);
            teamBLifeSupportSystemTransform.position = LSSRespawnPoints[candidatePoint];
        }
    }

    void SetPoint(ref Vector3 teamTransformPosition)
    {
        int candidatePoint = UnityEngine.Random.Range(0, LSSRespawnPoints.Count);
        teamTransformPosition = LSSRespawnPoints[candidatePoint];
    }
}
