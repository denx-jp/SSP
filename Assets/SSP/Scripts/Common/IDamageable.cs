using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

interface IDamageable<T>
{
    void SetDamage(Damage damage);
    Subject<T> GetDeathStream();
}
