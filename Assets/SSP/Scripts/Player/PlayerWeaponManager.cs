using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerWeaponManager : NetworkBehaviour
{
    private PlayerInputManager pim;
    private PlayerAnimationController animationController;
    public IWeapon weapon;

    void Start()
    {
        pim = GetComponent<PlayerInputManager>();
        animationController = GetComponent<PlayerAnimationController>();

        pim.AttackButtonShort
            .Where(input => input)
            .Where(_ => weapon != null)
            .Subscribe(_ =>
            {
                weapon.NormalAttack();
            });

        pim.AttackButtonLong
            .Where(_ => weapon != null)
            .Subscribe(input =>
            {
                weapon.NormalAttackLong(input);
            });

        pim.ScopeButtonShort
           .Where(input => input)
           .Where(_ => weapon != null)
           .Subscribe(_ =>
           {
               weapon.SwitchScope();
           });

        pim.ScopeButtonLong
            .Where(_ => weapon != null)
            .Subscribe(input =>
            {
                weapon.LongPressScope(input);
            });
    }
}
