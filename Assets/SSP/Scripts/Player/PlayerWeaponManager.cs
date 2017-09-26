using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerWeaponManager : MonoBehaviour
{
    private Animator animator;
    private IAttackable weapon;
    private PlayerInputManager pim;

    void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();

        pim.NormalAttackButtonDown
            .Where(input => input)
            .Where(_ => weapon != null)
            .Subscribe(_ =>
            {
                weapon.NormalAttack(animator);
            });
    }

    public void SetAttacker(IAttackable atk)
    {
        weapon = atk;
    }
}
