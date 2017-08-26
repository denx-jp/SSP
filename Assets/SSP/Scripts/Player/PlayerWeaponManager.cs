using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerWeaponManager : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private WeaponAttacker weaponAttacker;
    private PlayerInputManager pim;

    void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();

        pim.NormalAttackButtonDown
            .Where(input => input)
            .Subscribe(_ =>
            {
                weaponAttacker.NormalAttack(animator);
            });
    }

}
