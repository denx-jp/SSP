using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public int id; //攻撃側のプレイヤーのID
    public float amount; //ダメージ量

    public Damage(float a,int _id)
    {
        amount = a;
        id = _id;
    }
}

