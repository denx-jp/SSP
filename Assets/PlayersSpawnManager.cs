using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersSpawnManager : NetworkManager
{
    [SerializeField] private List<GameObject> points;
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var playerSpawnPos = new Vector3(0, 0, 0);
        var player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
