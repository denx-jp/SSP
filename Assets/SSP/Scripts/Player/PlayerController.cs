using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

//移動やジャンプにおけるTransform・RigidBodyの操作など
public class PlayerController : MonoBehaviour
{
    private PlayerLocomotor locomotor;

    private Transform cameraTransform;
    private bool isJumping;
    private bool isDashing;

    private enum MoveMode { normal, battle };
    private MoveMode mode;

    void Start()
    {
        var input = GetComponent<OfflineInput>();
        locomotor = GetComponent<PlayerLocomotor>();
        cameraTransform = Camera.main.transform;
        mode = MoveMode.normal;

        input.Move
            .Subscribe(v =>
            {
                Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                Vector3 moveDir = v.y * cameraForward + v.x * cameraTransform.right;

                locomotor.Move(moveDir, isDashing);
                if (mode == MoveMode.normal)
                    locomotor.RotateTowardsMovementDir(moveDir);
                else
                    locomotor.RotateCameraDir(cameraTransform.forward);
            });

        input.JumpButtonDown
            .Where(v => v)
            .Subscribe(v => locomotor.Jump());

        input.DashButtonDown
            .Subscribe(v => isDashing = v);

        input.ScopeButtonLong
            .Subscribe(_ =>
            {
                SwitchMoveMode();
            });
    }

    public void SwitchMoveMode()
    {
        mode = mode == MoveMode.normal ? MoveMode.battle : MoveMode.normal;
    }
}
