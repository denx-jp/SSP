using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayersAroundLSSSpawner : NetworkManager
{
    [SerializeField]private PlayersSpawnAroundLSSManager playersSpawnAroundLSSManager; 

    void Start()
    {
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var playerTeamId = playerPrefab.GetComponent<PlayerModel>().teamId;

        foreach (var spawnPositionList in playersSpawnAroundLSSManager.spawnPointsDic)
        {
            var lssTeamId = spawnPositionList.Key.GetComponent<LifeSupportSystemModel>().GetTeamId();
            if(playerTeamId == lssTeamId)
                Debug.Log("I found the target");
        }

        var playerSpawnPos = new Vector3(0, 0, 0);
        var player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
