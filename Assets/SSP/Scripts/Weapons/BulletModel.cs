using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletModel : NetworkBehaviour
{
    [HideInInspector, SyncVar] public int shootPlayerId, shootPlayerTeamId;
    [HideInInspector, SyncVar] public float damageAmount, deathTime;
    public bool isShooterLocalPlayer;

    public void SetProperties(int _shootPlayerId, int _shootPlayerTeamId, float _damageAmount, float _deathTime)
    {
        shootPlayerId = _shootPlayerId;
        shootPlayerTeamId = _shootPlayerTeamId;
        damageAmount = _damageAmount;
        deathTime = _deathTime;
    }

    public void SetProperties(LongRangeWeaponModel lrwm)
    {
        shootPlayerId = lrwm.playerId;
        shootPlayerTeamId = lrwm.teamId;
        damageAmount = lrwm.damageAmount;
        deathTime = lrwm.bulletDeathTime;
        isShooterLocalPlayer = lrwm.isOwnerLocalPlayer;
    }
}
