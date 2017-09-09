using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

interface IEther
{
    void SetEther(float ether);
    float GetEther();
    ReactiveProperty<float> GetEtherStream();
    void AcquireEther(float ether);
}
