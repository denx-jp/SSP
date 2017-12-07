using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public int AttackerPlayerId; //攻撃側のプレイヤーのID
    public int AttackerTeamId; // 攻撃側のチームID
    public float amount; //ダメージ量

    public Damage(float a, int _id, int _teamId)
    {
        amount = a;
        AttackerPlayerId = _id;
        AttackerTeamId = _teamId;
    }
}

