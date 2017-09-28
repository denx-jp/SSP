using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerWeaponManager : MonoBehaviour
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
            .Where(_ => ExistAttacker())
            .Subscribe(_ =>
            {
                attacker.NormalAttack(animator);
            });
    }

    public bool ExistAttacker()
    {
        return attacker != null;
    }
}
