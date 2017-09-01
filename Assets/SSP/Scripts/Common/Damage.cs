using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public float amount;
    public int id;

    public Damage(float a,int _id)
    {
        amount = a;
        id = _id;
    }
}

