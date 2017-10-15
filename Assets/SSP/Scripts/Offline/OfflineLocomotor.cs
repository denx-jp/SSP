using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityStandardAssets.Characters.ThirdPerson;

public class OfflineLocomotor : MonoBehaviour
{
    private PlayerController playerController;

    private Transform cameraTransform;
    private bool isJumping;
    private bool isDashing;

    void Start()
    {
        var input = GetComponent<OfflineInput>();
        playerController = GetComponent<PlayerController>();
        var tpc = GetComponent<ThirdPersonCharacter>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        input.Move
            .Subscribe(v =>
            {
                Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                Vector3 moveDir = v.y * cameraForward + v.x * cameraTransform.right;

                //playerController.Move(moveDir, isDashing);
                tpc.Move(moveDir, false, isJumping);
            });

        input.JumpButtonDown
            .Where(v => v)
            .Where(_ => playerController.isOnGround)
            .Subscribe(v => playerController.Jump());

        input.DashButtonDown
            .Subscribe(v => isDashing = v);
    }
}
