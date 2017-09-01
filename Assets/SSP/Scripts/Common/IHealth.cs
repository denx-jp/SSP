using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHealth
{
    void SetDamage(Damage damage);
    float GetHealth();
}
