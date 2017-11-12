using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class PlayerAnimationEventHandler : MonoBehaviour
{
    public readonly Subject<FootstepType> FootstepStream = new Subject<FootstepType>();

    [SerializeField] private float thresholdIdle;
    private Vector3 previousPosition = new Vector3();

    void FootR()
    {
        if (!IsPlayerMoving()) return;
        FootstepStream.OnNext(FootstepType.Right);
    }

    void FootL()
    {
        if (!IsPlayerMoving()) return;
        FootstepStream.OnNext(FootstepType.Left);
    }

    void Hit() { }
    void Land()
    {
        FootstepStream.OnNext(FootstepType.Landing);
        // 速度制限処理使予定
    }
    void WeaponSwitch() { }
    void Shoot() { }

    bool IsPlayerMoving()
    {
        var velocity = (this.transform.position - previousPosition).sqrMagnitude;
        previousPosition = this.transform.position;
        return velocity > thresholdIdle;
    }
}
