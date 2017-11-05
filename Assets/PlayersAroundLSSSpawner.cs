using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersAroundLSSSpawner : NetworkManager
{
    [SerializeField]private PlayersSpawnAroundLSSManager playersSpawnAroundLSSManager;
    private Vector3 playerSpawnPosition;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var playerTeamId = playerPrefab.GetComponent<PlayerModel>().teamId;

        foreach (var spawnPositionList in playersSpawnAroundLSSManager.spawnPointsDic)
        {
            var lssTeamId = spawnPositionList.Key.GetComponent<LifeSupportSystemModel>().GetTeamId();
            if(playerTeamId == lssTeamId)
                SetSpawnPosition(spawnPositionList.Value);
        }

        var player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    private void SetSpawnPosition(List<Transform> _spawnPositionList)
    {
        int candidatePoint = UnityEngine.Random.Range(0, _spawnPositionList.Count - 1);

        playerSpawnPosition = _spawnPositionList[candidatePoint].position;
        _spawnPositionList.Remove(_spawnPositionList[candidatePoint]);
    }
}
