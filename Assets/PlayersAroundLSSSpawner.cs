using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayersAroundLSSSpawner : NetworkManager
{
    public List<Transform> playerSpawnPointsAroundLSS;

    void Start()
    {
        playerSpawnPointsAroundLSS = new List<Transform>();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var playerSpawnPos = new Vector3(0, 0, 0);
        var player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
