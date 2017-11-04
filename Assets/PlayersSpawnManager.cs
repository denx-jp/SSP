using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersSpawnManager : NetworkManager
{
    [SerializeField] private List<Transform> LifeSupportSystemObjects;
    [SerializeField, Range(1.0f, 5.0f)] private float distance;

    private Dictionary<int,List<Transform>> spawnPointsDic;

    void Start()
    {
        spawnPointsDic = new Dictionary<int, List<Transform>>();
    }

    public void Init()
    {
        foreach (var lss in LifeSupportSystemObjects)
            InitSpawnPoints(lss);
    }
    private void InitSpawnPoints(Transform lss)
    {
        var teamId = lss.GetComponent<LifeSupportSystemModel>().GetTeamId();
        var spawnPoints = new List<Transform>();

        foreach (Transform spawnPoint in lss)
            spawnPoints.Add(spawnPoint);

        spawnPointsDic.Add(teamId, spawnPoints);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var playerSpawnPos = new Vector3(0, 0, 0);
        var player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
