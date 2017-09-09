using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

interface IHealth
{
    float GetHealth();
    ReactiveProperty<float> GetHealthStream();
}
