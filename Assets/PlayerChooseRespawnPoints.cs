using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerChooseRespawnPoints : MonoBehaviour {

    public GameObject LSS;
    private PlayerInputManager pim;
    [SerializeField] private List<GameObject> RespawnPoints = new List<GameObject>();
    [SerializeField] private float Range;

    void Start () {
        pim = GetComponent<PlayerInputManager>();

        for (int i = 0; i < RespawnPoints.Count; i++)
        {
            float Distance = (LSS.transform.position - RespawnPoints[i].transform.position).sqrMagnitude;
            // LSSとリスポーン地点の二点間の距離をとる
            if (Distance > Range)
            {
                RespawnPoints.RemoveAt(i);
            }
        }
        pim.ChooseRespawnPointsButtonDown
            .Where(v => v)
            .Where(v => Input.GetButtonDown("Right"))
            .Subscribe(v =>
            {
                 
            });
        pim.ChooseRespawnPointsButtonDown
            .Where(v => v)
            .Where(v => Input.GetButtonDown("Left"))
            .Subscribe(v =>
            {
                //実行内容
            });
    }
    void Update()
    {
        //for (int i = 0; i < RespawnPoints.Count ; i++){
        //    float Distance = (LSS.transform.position - RespawnPoints[i].transform.position).sqrMagnitude;
        //    // LSSとリスポーン地点の二点間の距離をとる
        //   if(Distance > Range)
        //    {
        //        RespawnPoints.RemoveAt(i);
        //    }
        //}
    }
}
