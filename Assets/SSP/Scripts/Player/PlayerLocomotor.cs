using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerLocomotor : MonoBehaviour
{
    private ThirdPersonCharacter thirdPersonCharacter;
    private Transform cameraTransform;
    private Vector3 cameraForward;
    private Vector3 m_Move;
    [SerializeField] private float dashSpeed = 2f;
    private bool isJumping;
    private bool isDashing;
    private PlayerInputManager pim;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        pim = GetComponent<PlayerInputManager>();

        pim.Move
            .Subscribe(input =>
            {
                if (cameraTransform != null)
                {
                    cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                    m_Move = input.y * cameraForward + input.x * cameraTransform.right;
                }
                else
                {
                    m_Move = input.y * Vector3.forward + input.x * Vector3.right;
                }

                if (isDashing)
                    m_Move *= dashSpeed;

                thirdPersonCharacter.Move(m_Move * 0.5f, false, isJumping);        //しゃがむのは仕様にないのでcrouchの部分はとりあえずfalseで
                isJumping = false;
            });

        pim.JumpButtonDown
            .Where(v => v)
            .Subscribe(input =>
            {
                if (!isJumping)
                {
                    isJumping = input;
                }
            });

        pim.DashButtonDown
            .Subscribe(input =>
            {
                isDashing = input;
            });
    }
}
