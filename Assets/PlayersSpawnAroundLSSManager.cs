using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayersSpawnAroundLSSManager : MonoBehaviour
{
    [SerializeField] private List<Transform> LifeSupportSystemObjects;
    [SerializeField, Range(1.0f, 5.0f)] private float distance;

    private Dictionary<Transform,List<Transform>> spawnPointsDic;

    public void Init()
    {
        spawnPointsDic = new Dictionary<Transform, List<Transform>>();

        foreach (var lss in LifeSupportSystemObjects)
            InitSpawnPoints(lss);
    }

    void InitSpawnPoints(Transform lss)
    {
        ///var teamId = lss.GetComponent<LifeSupportSystemModel>().GetTeamId();
        var spawnPoints = new List<Transform>();

        foreach (Transform spawnPoint in lss)
            spawnPoints.Add(spawnPoint);

        spawnPointsDic.Add(lss, spawnPoints);
    }
}