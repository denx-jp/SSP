using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerModel player = gamePlayer.GetComponent<PlayerModel>();

        player.name = lobby.playerName;
        player.playerId = lobby.playerId;
        player.teamId = lobby.teamId;
    }
}
