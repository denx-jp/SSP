using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerWeaponManager : NetworkBehaviour
{
    private Animator animator;
    private PlayerInputManager pim;

    public IAttackable attacker;

    private Transform cameraTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();
        cameraTransform = Camera.main.transform;

        pim.NormalAttackButtonDown
            .Where(input => input)
            .Where(_ => attacker != null)
            .Subscribe(_ =>
            {
                CmdAttack(cameraTransform.position, cameraTransform.forward, cameraTransform.rotation);
            });
    }

    [Command]
    private void CmdAttack(Vector3 camPos, Vector3 camDir, Quaternion camRot)
    {
        RpcAttack(camPos, camDir, camRot);
    }

    [ClientRpc]
    private void RpcAttack(Vector3 camPos, Vector3 camDir, Quaternion camRot)
    {
        attacker.NormalAttack(animator, camPos, camDir, camRot);

    }
}
