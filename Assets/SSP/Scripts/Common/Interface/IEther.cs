using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IEther
{
    float GetEther();
    float GetMaxEther();
    ReactiveProperty<float> GetEtherStream();
}
