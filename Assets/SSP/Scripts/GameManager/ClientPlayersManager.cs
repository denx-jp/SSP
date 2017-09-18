using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayersManager : MonoBehaviour
{
    [SerializeField] private List<PlayerManager> playerManagers;

    public void AddPlayer(PlayerManager pm)
    {
        playerManagers.Add(pm);
    }
}
