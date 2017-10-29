using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ClientPlayersManager : MonoBehaviour
{
    public static ClientPlayersManager Instance;
    public static List<PlayerManager> Players = new List<PlayerManager>();

    private void Awake()
    {
        Instance = this;
        Players = new List<PlayerManager>();
    }

    public static void AddPlayer(PlayerManager pm)
    {
        Players.Add(pm);
    }

    public List<T> GetPlayersComponent<T>()
    {
        if (typeof(T) == typeof(PlayerModel))
            return Players.Select(v => (T)(object)v.playerModel).ToList<T>();
        else if (typeof(T) == typeof(PlayerHealthManager))
            return Players.Select(v => (T)(object)v.playerHealthManager).ToList<T>();
        else if (typeof(T) == typeof(PlayerEtherManager))
            return Players.Select(v => (T)(object)v.playerEtherManager).ToList<T>();
        else if (typeof(T) == typeof(PlayerInputManager))
            return Players.Select(v => (T)(object)v.playerInputManager).ToList<T>();
        else if (typeof(T) == typeof(PlayerKillLogNotifier))
            return Players.Select(v => (T)(object)v.playerKillLogNotifier).ToList<T>();
        else
            return default(List<T>);
    }
}
