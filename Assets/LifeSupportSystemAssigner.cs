using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class LifeSupportSystemAssigner : NetworkBehaviour
{
    [SerializeField]private Transform team1LifeSupportSystemTransform;
    [SerializeField]private Transform team2LifeSupportSystemTransform;

    private List<GameObject> LSSAssignPointObjectList;

    [Server]
    void Start()
    {
        LSSAssignPointObjectList = GameObject.FindGameObjectsWithTag(TagMap.Respawn).ToList();

        if (LSSAssignPointObjectList.Count >= 2)
        {
            SetPoint(team1LifeSupportSystemTransform);
            SetPoint(team2LifeSupportSystemTransform);
        }
    }

    void SetPoint(Transform LSSTransform)
    {
        int candidatePoint = UnityEngine.Random.Range(0, LSSAssignPointObjectList.Count - 1);

        LSSTransform.position = LSSAssignPointObjectList[candidatePoint].transform.position;
        LSSAssignPointObjectList.Remove(LSSAssignPointObjectList[candidatePoint]);
    }
}