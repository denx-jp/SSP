using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class PlayerAnimationEventHandler : MonoBehaviour
{
    public readonly Subject<FootstepType> FootstepStream = new Subject<FootstepType>();

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float threshold_idle;

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
        return Vector3.Dot(transform.forward, rb.velocity) > threshold_idle;
    }
}
