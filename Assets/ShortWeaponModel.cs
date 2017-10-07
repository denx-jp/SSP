using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortWeaponModel : MonoBehaviour
{
    public string weaponName;
    private int playerId, teamId;

    public void WeaponModel(PlayerModel playerModel)
    {
        playerId = playerModel.playerId;
        teamId = playerModel.teamId;
    }
}
