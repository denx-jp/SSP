using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LifeSupportSystemPositionManager : MonoBehaviour
{
    public static LifeSupportSystemPositionManager Instance;
    public static Dictionary<int, Transform> lifeSupportSystemPositionDic;
    public static Dictionary<int, List<Transform>> spawnablePositionDic;

    [SerializeField] private List<Transform> lifeSupportSystemTransforms;
    [SerializeField] private float spawnableDistance;

    private List<GameObject> spawnPositionObjectList;

    private void Awake()
    {
        Instance = this;

        lifeSupportSystemPositionDic = new Dictionary<int, Transform>();
        spawnablePositionDic = new Dictionary<int, List<Transform>>();

        spawnPositionObjectList =
            GameObject.FindGameObjectsWithTag(TagMap.Respawn).ToList();
    }

    public void UpdateLSSPositionDic(Transform _LSSTransform)
    {
        var LSSTeamId = _LSSTransform.GetComponent<LifeSupportSystemModel>().GetTeamId();
        lifeSupportSystemPositionDic[LSSTeamId] = _LSSTransform;

        SetSpawnablePosition(LSSTeamId);
    }

    private void SetSpawnablePosition(int _teamId)
    {
        spawnablePositionDic[_teamId].Clear();

        float distance;
        foreach(var spawnPosition in spawnPositionObjectList)
        {
            distance = 
                Vector3.Distance(lifeSupportSystemPositionDic[_teamId].position,
                                spawnPosition.transform.position);

            if (distance <= spawnableDistance)
                spawnablePositionDic[_teamId].Add(spawnPosition.transform);
        }
    }

    public Transform GetSpawnPosition(int _teamId)
    {
        var spawnablePositionList = GetSpawnablePositionList(_teamId);
        if (spawnablePositionList == null) return null;

        int candidatePoint = UnityEngine.Random.Range(0, spawnablePositionList.Count-1);
        var spawnPosition = spawnablePositionList[candidatePoint];

        spawnablePositionList.Remove(spawnPosition);
        return spawnPosition;
    }

    private List<Transform> GetSpawnablePositionList(int _teamId)
    {
        if (spawnablePositionDic[_teamId].Count == 0)
            return null;
        return spawnablePositionDic[_teamId];
    }
}
