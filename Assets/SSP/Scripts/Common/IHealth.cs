using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

interface IHealth
{
    void SetDamage(Damage damage);
    float GetHealth();
    Subject<bool> GetDeathStream();
}
