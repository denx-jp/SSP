using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class SpawnablePositionManager : MonoBehaviour
{
    public static SpawnablePositionManager Instance;
    public static Dictionary<int, Transform> lifeSupportSystemTransformDic;
    public static Dictionary<int, List<Transform>> spawnablePositionDic;

    [SerializeField] private List<Transform> lifeSupportSystemTransforms;
    [SerializeField] private float spawnableDistance;
    [SerializeField] private float lssToSpawnPositionDisableDistance;

    private List<GameObject> spawnPositionObjectList;

    private void Awake()
    {
        Instance = this;

        lifeSupportSystemTransformDic = new Dictionary<int, Transform>();
        spawnablePositionDic = new Dictionary<int, List<Transform>>();

        spawnPositionObjectList =
            GameObject.FindGameObjectsWithTag(TagMap.Respawn).ToList();

        foreach (var lssTransform in lifeSupportSystemTransforms)
            this.ObserveEveryValueChanged(_ => lssTransform)
                .Subscribe(v => UpdateLSSPositionDic(v));
    }

    public void UpdateLSSPositionDic(Transform _LSSTransform)
    {
        var LSSTeamId = _LSSTransform.GetComponent<LifeSupportSystemModel>().GetTeamId();
        lifeSupportSystemTransformDic[LSSTeamId] = _LSSTransform;

        SetSpawnablePosition(LSSTeamId);
    }
    private void SetSpawnablePosition(int _teamId)
    {
        // 連想配列の値を強制的に上書きするためにListを使用
        List<Transform> spawnablePositions = new List<Transform>();

        float distance;
        foreach(var spawnPosition in spawnPositionObjectList)
        {
            distance = 
                Vector3.Distance(lifeSupportSystemTransformDic[_teamId].position,
                                spawnPosition.transform.position);

            if (distance <= spawnableDistance)
                spawnablePositions.Add(spawnPosition.transform);
        }
        spawnablePositionDic[_teamId] = spawnablePositions;
    }

    public Transform GetSpawnPosition(int _teamId)
    {
        var spawnablePositionList = GetSpawnablePositionList(_teamId);
        if (spawnablePositionList == null) return null;

        var candidatePoint = UnityEngine.Random.Range(0, spawnablePositionList.Count-1);
        var spawnTransform = spawnablePositionList[candidatePoint];

        // lssがスポーン位置近くにあったときにスポーン位置を少しずらす
        Transform lssTransform = lifeSupportSystemTransformDic[_teamId];
        if(Vector3.Distance(lssTransform.position, spawnTransform.position)
            <= lssToSpawnPositionDisableDistance)
        {
            spawnTransform.position -= 3.0f * (lssTransform.position - spawnTransform.position);
        }

        return spawnTransform;
    }
    private List<Transform> GetSpawnablePositionList(int _teamId)
    {
        if (spawnablePositionDic[_teamId].Count == 0)
            return null;
        return spawnablePositionDic[_teamId];
    }

    public void Init()
    {
        foreach(var lssTransform in lifeSupportSystemTransforms)
            UpdateLSSPositionDic(lssTransform);
    }
}
