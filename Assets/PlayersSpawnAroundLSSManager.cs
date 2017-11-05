using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayersSpawnAroundLSSManager : MonoBehaviour
{
    [SerializeField] private List<Transform> LifeSupportSystemObjects;
    [SerializeField, Range(1.0f, 5.0f)] private float distance;

    public Dictionary<Transform,List<Transform>> spawnPointsDic;

    void Start()
    {
        spawnPointsDic = new Dictionary<Transform, List<Transform>>();

        this.ObserveEveryValueChanged(_ => distance)
            .Subscribe(v => SetSpawnPoints());
    }

    public void Init()
    {
        foreach (var lss in LifeSupportSystemObjects)
            InitSpawnPoints(lss);

        SetSpawnPoints();
    }

    void InitSpawnPoints(Transform lss)
    {
        var spawnPoints = new List<Transform>();

        foreach (Transform spawnPoint in lss)
            spawnPoints.Add(spawnPoint);

        spawnPointsDic.Add(lss,spawnPoints);
    }

    private void SetSpawnPoints()
    {
        foreach(var spawnPositionList in spawnPointsDic)
            CalcSpawnPoints(spawnPositionList);
    }

    private void CalcSpawnPoints(KeyValuePair<Transform,List<Transform>> LSSSpawnPoints)
    {
        var LSSPosition = LSSSpawnPoints.Key.position;

        foreach (var spawnPoint in LSSSpawnPoints.Value)
        {
            Vector3 relative = spawnPoint.position - LSSPosition;
            float rad = Mathf.Atan2(relative.z, relative.x);

            Vector3 newPosition =
                new Vector3(distance * Mathf.Cos(rad), 0, distance * Mathf.Sin(rad));

            spawnPoint.position = LSSPosition + newPosition;
        }
    }
}