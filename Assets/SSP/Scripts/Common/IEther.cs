using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IEther
{
    void SetEther(float ether);
    float GetEther();
    void AcquireEther(float ether);
    void ReduceEther(float ether);
}
