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

    private GameObject[] LSSAssignPointArray;
    private List<GameObject> LSSAssignPointList;

    [Server]
    void Start()
    {
        LSSAssignPointArray = GameObject.FindGameObjectsWithTag(TagMap.Respawn);
        LSSAssignPointList = new List<GameObject>(LSSAssignPointArray);

        if (LSSAssignPointList.Count >= 2)
        {
            SetPoint(team1LifeSupportSystemTransform);
            SetPoint(team2LifeSupportSystemTransform);
        }
    }

    void SetPoint(Transform LSSTransform)
    {
        int candidatePoint = UnityEngine.Random.Range(0, LSSAssignPointList.Count - 1);

        LSSTransform.position = LSSAssignPointList[candidatePoint].transform.position;
        LSSAssignPointList.Remove(LSSAssignPointList[candidatePoint]);
    }
}