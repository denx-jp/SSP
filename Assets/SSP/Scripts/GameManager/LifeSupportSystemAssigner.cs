using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class LifeSupportSystemAssigner : NetworkBehaviour
{
    [SerializeField] private List<Transform> LSSTransforms;
    [SerializeField] private Vector3 offset;

    private List<GameObject> LSSAssignPointObjectList;

    [ServerCallback]
    void Start()
    {
        LSSAssignPointObjectList = GameObject.FindGameObjectsWithTag(TagMap.Respawn).ToList();

        if (LSSAssignPointObjectList.Count >= 2)
        {
            foreach(var LSSTransform in LSSTransforms)
                SetLSSStartPosition(LSSTransform);
        }
    }

    private void SetLSSStartPosition(Transform _LSSTransform)
    {
        int candidatePoint = UnityEngine.Random.Range(0, LSSAssignPointObjectList.Count - 1);

        _LSSTransform.position = LSSAssignPointObjectList[candidatePoint].transform.position + offset;
        LSSAssignPointObjectList.Remove(LSSAssignPointObjectList[candidatePoint]);
    }
}