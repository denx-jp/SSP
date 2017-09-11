using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IHealth
{
    float GetHealth();
    ReactiveProperty<float> GetHealthStream();
}
