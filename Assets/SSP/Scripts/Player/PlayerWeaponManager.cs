using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerWeaponManager : MonoBehaviour
{
    private Animator animator;
    private IAttackable attacker;
    private PlayerInputManager pim;

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

    public void SetAttacker(IAttackable atk)
    {
        attacker = atk;
    }

    public bool ExistAttacker()
    {
        return attacker != null;
    }
}
