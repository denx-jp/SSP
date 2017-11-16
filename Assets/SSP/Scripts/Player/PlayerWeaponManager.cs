using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerWeaponManager : NetworkBehaviour
{
    private PlayerModel model;
    private PlayerInputManager pim;
    private PlayerAnimationController animationController;
    public IWeapon weapon;

    void Start()
    {
        model = GetComponent<PlayerModel>();
        pim = GetComponent<PlayerInputManager>();
        animationController = GetComponent<PlayerAnimationController>();

        pim.AttackButtonShort
            .Where(input => input)
            .Where(_ => weapon != null)
            .Where(_ => model.MoveMode != MoveMode.carry)
            .Subscribe(_ =>
            {
                weapon.NormalAttack();
            });

        pim.AttackButtonLong
            .Where(_ => weapon != null)
            .Where(_ => model.MoveMode != MoveMode.carry)
            .Subscribe(input =>
            {
                weapon.NormalAttackLong(input);
            });

        pim.ScopeButtonShort
           .Where(input => input)
           .Where(_ => weapon != null)
            .Where(_ => model.MoveMode != MoveMode.carry)
           .Subscribe(_ =>
           {
               weapon.SwitchScope();
           });

        pim.ScopeButtonLong
            .Where(_ => weapon != null)
            .Where(_ => model.MoveMode != MoveMode.carry)
            .Subscribe(input =>
            {
                weapon.LongPressScope(input);
            });
    }
}
