using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSupportSystemPositionManager : MonoBehaviour
{
    public static LifeSupportSystemPositionManager Instance;
    public static Dictionary<int, Transform> LSSPositionDic;

    private void Awake()
    {
        Instance = this;
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
