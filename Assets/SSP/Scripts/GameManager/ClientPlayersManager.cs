using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class ClientPlayersManager : NetworkBehaviour
{
    public static ClientPlayersManager Instance;
    [SerializeField] public List<PlayerManager> playerManagers { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerManagers = new List<PlayerManager>();
    }

    public static void AddPlayer(GameObject player)
    {
        Instance.playerManagers.Add(player.GetComponent<PlayerManager>());
    }

    public List<T> GetPlayersComponent<T>()
    {
        if (typeof(T) == typeof(PlayerModel))
            return playerManagers.Select(v => (T)(object)v.playerModel).ToList<T>();
        else if (typeof(T) == typeof(PlayerHealthManager))
            return playerManagers.Select(v => (T)(object)v.playerHealthManager).ToList<T>();
        else if (typeof(T) == typeof(PlayerEtherManager))
            return playerManagers.Select(v => (T)(object)v.playerEtherManager).ToList<T>();
        else if (typeof(T) == typeof(PlayerInputManager))
            return playerManagers.Select(v => (T)(object)v.playerInputManager).ToList<T>();
        else if (typeof(T) == typeof(PlayerKillLogNotifier))
            return playerManagers.Select(v => (T)(object)v.playerKillLogNotifier).ToList<T>();
        else
            return default(List<T>);
    }
}
