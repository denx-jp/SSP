using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerWeaponManager : NetworkBehaviour
{
    private PlayerInputManager pim;
    public IWeapon weapon;

    void Start()
    {
        pim = GetComponent<PlayerInputManager>();

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

        pim.ScopeButtonLong
            .Where(_ => weapon != null)
            .Subscribe(input =>
            {
                weapon.LongPressScope(input);
            });
    }
}
