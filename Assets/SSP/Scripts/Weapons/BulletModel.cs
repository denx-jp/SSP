using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletModel : MonoBehaviour
{
    public int shootPlayerId, shootPlayerTeamId;
    public float damageAmount, deathTime;

    public void SetProperties(int _shootPlayerId, int _shootPlayerTeamId, float _damageAmount, float _deathTime)
    {
        shootPlayerId = _shootPlayerId;
        shootPlayerTeamId = _shootPlayerTeamId;
        damageAmount = _damageAmount;
        deathTime = _deathTime;
    }
}
