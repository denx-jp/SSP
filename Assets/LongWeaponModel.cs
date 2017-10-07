using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongWeaponModel : MonoBehaviour {
    public string weaponName ;
    private BulletModel bulletModel;
    private int playerId, teamId;

    public void WeaponModel(PlayerModel playerModel)
    {
        playerId = playerModel.playerId;
        teamId = playerModel.teamId;
    }

}
