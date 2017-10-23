﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public int playerId; //攻撃側のプレイヤーのID
    public int teamId; // 攻撃側のチームID
    public float amount; //ダメージ量

    public Damage(float a,int _id, int _teamId)
    {
        amount = a;
        playerId = _id;
        teamId = _teamId;
    }
}

