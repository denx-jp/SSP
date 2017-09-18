using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ClientPlayersManager : MonoBehaviour
{
    [SerializeField] public List<PlayerManager> playerManagers { get; private set; }

    private void Awake()
    {
        playerManagers = new List<PlayerManager>();
    }

    public void AddPlayer(PlayerManager pm)
    {
        playerManagers.Add(pm);
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
