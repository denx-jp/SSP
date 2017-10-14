using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityStandardAssets.Characters.ThirdPerson;

public class OfflineLocomotor : MonoBehaviour
{

    private ThirdPersonCharacter thirdPersonCharacter;
    private Transform cameraTransform;
    private Vector3 cameraForward;
    private Vector3 m_Move;
    [SerializeField] private float dashSpeed = 2f;
    private bool isJumping;
    private bool isDashing;

    void Start()
    {
        var input = GetComponent<OfflineInput>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();

        input.Move
            .Subscribe(v =>
            {
                if (cameraTransform != null)
                {
                    cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                    m_Move = v.y * cameraForward + v.x * cameraTransform.right;
                }
                else
                {
                    m_Move = v.y * Vector3.forward + v.x * Vector3.right;
                }

                if (isDashing)
                    m_Move *= dashSpeed;

                thirdPersonCharacter.Move(m_Move * 0.5f, false, isJumping);        //しゃがむのは仕様にないのでcrouchの部分はとりあえずfalseで
                isJumping = false;
            });

        input.JumpButtonDown
            .Where(v => v)
            .Where(_ => !isJumping)
            .Subscribe(v => isJumping = v);

        input.DashButtonDown
            .Subscribe(v => isDashing = v);
    }
}
