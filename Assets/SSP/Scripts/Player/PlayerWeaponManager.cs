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
    
    void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();

        pim.NormalAttackButtonDown
            .Where(input => input)
            .Where(_ => attacker != null)
            .Subscribe(_ =>
            {
                attacker.NormalAttack(animator);
            });
    }

    [Command]
    private void CmdAttack()
    {
        RpcAttack();
    }

    [ClientRpc]
    private void RpcAttack()
    {
        attacker.NormalAttack(animator);
    }
}
