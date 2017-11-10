using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class PlayerAnimationEventHandler : MonoBehaviour
{
    public readonly Subject<FootstepType> FootstepStream = new Subject<FootstepType>();

    void FootR()
    {
        FootstepStream.OnNext(FootstepType.R);
    }

    void FootL()
    {
        FootstepStream.OnNext(FootstepType.L);
    }

    void Hit() { }
    void Land()
    {
        // 速度制限処理使予定
    }
    void WeaponSwitch() { }
    void Shoot() { }
}
