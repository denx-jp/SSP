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
    private List<GameObject> LSSAssignPointObjectList;
    public Dictionary<int, Transform> LSSPositionDic;

    [ServerCallback]
    void Start()
    {
        LSSPositionDic = new Dictionary<int, Transform>();
        LSSAssignPointObjectList = GameObject.FindGameObjectsWithTag(TagMap.Respawn).ToList();

        if (LSSAssignPointObjectList.Count >= 2)
        {
            foreach(var LSSTransform in LSSTransforms)
                SetPoint(LSSTransform);

            UpdateLSSPositionDic();
        }
    }

    void SetPoint(Transform _LSSTransform)
    {
        int candidatePoint = UnityEngine.Random.Range(0, LSSAssignPointObjectList.Count - 1);

        _LSSTransform.position = LSSAssignPointObjectList[candidatePoint].transform.position;
        LSSAssignPointObjectList.Remove(LSSAssignPointObjectList[candidatePoint]);
    }

    public void UpdateLSSPositionDic()
    {
        foreach(var LSSTransform in LSSTransforms)
        {
            var LSSTeamId = LSSTransform.GetComponent<LifeSupportSystemModel>().GetTeamId();
            LSSPositionDic[LSSTeamId] = LSSTransform;
        }
    }
}