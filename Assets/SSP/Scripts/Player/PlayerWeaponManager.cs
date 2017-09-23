using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerWeaponManager : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject weapon;
    private IAttackable attacker;
    private PlayerInputManager pim;

    void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();
        SetAttacker(weapon.GetComponent<IAttackable>());

        pim.NormalAttackButtonDown
            .Where(input => input)
            .Subscribe(_ =>
            {
                attacker.NormalAttack(animator);
            });
    }

    public void SetAttacker(IAttackable atk)
    {
        attacker = atk;
    }
}
