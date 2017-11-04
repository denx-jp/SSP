using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayersSpawnAroundLSSManager : MonoBehaviour
{
    [SerializeField] private List<Transform> LifeSupportSystemObjects;
    [SerializeField, Range(1.0f, 5.0f)] private float distance;

    private Dictionary<Transform,List<Transform>> spawnPointsDic;

    void Start()
    {
        spawnPointsDic = new Dictionary<Transform, List<Transform>>();

        this.ObserveEveryValueChanged(_ => distance)
            .Subscribe(v => SetSpawnPoints(v));
    }

    public void Init()
    {
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

        SetSpawnPoints(1.0f);
    }

    private void SetSpawnPoints(float distance)
    {
        foreach(var pointPair in spawnPointsDic)
        {
            foreach(var point in pointPair.Value)
            {
                Vector3 relative = pointPair.Key.position - point.position;
                Vector3.Normalize(relative);
                float angle = 90 - Vector3.Angle(relative, transform.forward);

                Vector3 newPosition = 
                    new Vector3(distance * Mathf.Sin(Mathf.Deg2Rad*angle),
                                0,
                                distance * Mathf.Cos(Mathf.Deg2Rad * angle));

                point.transform.position = pointPair.Key.position + newPosition;

                Debug.Log(pointPair.Key.position + " " + point.position + " " + angle);
            }
        }
    }
}