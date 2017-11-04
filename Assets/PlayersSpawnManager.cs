using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersSpawnManager : NetworkManager
{
    [SerializeField] private List<Transform> LifeSupportSystemObjects;

    private Dictionary<GameObject, int> spawnPoints;

    void Start()
    {
        spawnPoints = new Dictionary<GameObject, int>();
    }

    public void Init()
    {
        foreach (var lss in LifeSupportSystemObjects)
            SetSpawnPoints(lss);
    }

    private void SetSpawnPoints(Transform lss)
    {
        var teamId = lss.GetComponent<LifeSupportSystemModel>().GetTeamId();
        foreach (Transform spawnPoint in lss)
            spawnPoints.Add(spawnPoint.gameObject, teamId);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var playerSpawnPos = new Vector3(0, 0, 0);
        var player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
