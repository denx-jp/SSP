using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public enum MoveMode { normal, battle, carry };
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerModel model;
    [SerializeField] private PlayerLocomotor locomotor;
    [SerializeField] private PlayerInputManager pim;

    private Transform cameraTransform;
    private bool isDashing;

    void Start()
    {
        cameraTransform = Camera.main.transform;

        pim.Move
            .Subscribe(v =>
            {
                Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                Vector3 moveDir = v.y * cameraForward + v.x * cameraTransform.right;

                locomotor.Move(moveDir, isDashing, model.MoveMode);
                if (model.MoveMode == MoveMode.normal || model.MoveMode == MoveMode.carry)
                    locomotor.RotateTowardsMovementDir(moveDir);
                else
                    locomotor.RotateCameraDir(cameraTransform.forward);
            });

        pim.JumpButtonDown
            .Where(v => v)
            .Subscribe(v => locomotor.Jump());

        pim.DashButtonDown
            .Subscribe(v => isDashing = v && model.MoveMode == MoveMode.normal);  // 移動モード時のみダッシュ可能
    }
}
