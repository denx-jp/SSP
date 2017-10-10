using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletModel : NetworkBehaviour
{
    [SyncVar] public int shootPlayerId, shootPlayerTeamId;
    [SyncVar] public float damageAmount, deathTime;

    public void SetProperties(int _shootPlayerId, int _shootPlayerTeamId, float _damageAmount, float _deathTime)
    {
        shootPlayerId = _shootPlayerId;
        shootPlayerTeamId = _shootPlayerTeamId;
        damageAmount = _damageAmount;
        deathTime = _deathTime;
    }
}
