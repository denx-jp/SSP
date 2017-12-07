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

        spawnPointsAroundLSS[1] = new List<Transform>();
        spawnPointsAroundLSS[2] = new List<Transform>();
    }

    public Vector3 GetRandomSpawnPosition()
    {
        var spawnPointIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[spawnPointIndex].position;
    }

    public Vector3 GetSpawnPositionAroundLSS(int teamId)
    {
        if (teamId != 1 && teamId != 2) return GetRandomSpawnPosition();   // デバッグ用

        var lssTransform = teamId == 1 ? team1LSS : team2LSS;

        RaycastHit hit;
        float downDirection = 1;
        while (true)
        {
            var x = lssTransform.position.x + GetRandomDistanceInRange();
            var z = lssTransform.position.z + GetRandomDistanceInRange();
            var y = lssTransform.position.y + 0.5f;
            var pos = new Vector3(x, y, z);

            if (Physics.Raycast(pos, Vector3.down, out hit, downDirection))
                return hit.point;

            downDirection *= 2;
        }
    }

    private void UpdateSpawnPointsAroundLSS(int teamId)
    {
        var lssTransform = teamId == 1 ? team1LSS : team2LSS;
        lssPrePosEachLss[teamId] = lssTransform.position;
        spawnPointsAroundLSS[teamId].Clear();

        foreach (var spawnPosition in spawnPoints)
        {
            var distance = Vector3.Distance(lssTransform.position, spawnPosition.position);

            if (nearLimitDistance < distance && distance < farLimitDistance)
                spawnPointsAroundLSS[teamId].Add(spawnPosition);
        }
    }

    private float GetRandomDistanceInRange()
    {
        float distance = Random.Range(nearLimitDistance, farLimitDistance);

        // 偶数が出たらマイナスにする
        if (Random.Range(0, 10) % 2 == 0)
            distance *= -1;

        return distance;
    }
}
