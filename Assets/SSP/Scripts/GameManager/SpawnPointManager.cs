using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance;

    [SerializeField] private float lssAroundPointUpdateThreshold;
    [SerializeField] private float farLimitDistance;
    [SerializeField] private float nearLimitDistance;

    private Transform team1LSS;
    private Transform team2LSS;
    private List<Transform> spawnPoints;
    private Dictionary<int, List<Transform>> spawnPointsAroundLSS = new Dictionary<int, List<Transform>>();
    private Dictionary<int, Vector3> lssPrePosEachLss = new Dictionary<int, Vector3>();

    private void Awake()
    {
        Instance = this;
        spawnPoints = GameObject.FindGameObjectsWithTag(TagMap.Respawn).Select(v => v.transform).ToList();
    }

    public void Init(Transform _team1Lss, Transform _team2Lss)
    {
        team1LSS = _team1Lss;
        team2LSS = _team2Lss;

        lssPrePosEachLss[1] = team1LSS.position;
        lssPrePosEachLss[2] = team2LSS.position;
    }

    public Transform GetRandomSpawnPoint()
    {
        var spawnPointIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[spawnPointIndex];
    }

    public Transform GetSpawnPointAroundLSS(int teamId)
    {
        if (teamId != 1 || teamId != 2) return GetRandomSpawnPoint();   // デバッグ用

        var lssTransform = teamId == 1 ? team1LSS : team2LSS;

        var distance = Vector3.Distance(lssTransform.position, lssPrePosEachLss[teamId]);
        if (distance > lssAroundPointUpdateThreshold)
            UpdateSpawnPointsAroundLSS(teamId);

        var spawnPointIndex = Random.Range(0, spawnPointsAroundLSS.Count);
        return spawnPointsAroundLSS[teamId][spawnPointIndex];
    }

    private void UpdateSpawnPointsAroundLSS(int teamId)
    {
        var lssTransform = teamId == 1 ? team1LSS : team2LSS;
        lssPrePosEachLss[teamId] = lssTransform.position;

        foreach (var spawnPosition in spawnPoints)
        {
            var distance = Vector3.Distance(lssTransform.position, spawnPosition.position);

            if (nearLimitDistance < distance && distance < farLimitDistance)
                spawnPointsAroundLSS[teamId].Add(spawnPosition);
        }
    }
}
