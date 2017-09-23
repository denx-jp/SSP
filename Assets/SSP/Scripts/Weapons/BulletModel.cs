using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletModel : MonoBehaviour
{
    public int shootPlayerId;
    public float damageAmount, deathTime;

    public void SetProperties(int pId, float dAmount, float dTime)
    {
        shootPlayerId = pId;
        damageAmount = dAmount;
        deathTime = dTime;
    }
}
