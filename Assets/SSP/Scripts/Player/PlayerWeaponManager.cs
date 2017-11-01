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

        pim.ScopeButtonLong
            .Subscribe(x =>
            {
                weapon.LongPressScope(x);
            });

        pim.AttackButtonShort
            .Where(input => input)
            .Where(_ => weapon != null)
            .Subscribe(_ =>
            {
                weapon.NormalAttack();
            });

        pim.AttackButtonLong
            .Where(_ => weapon != null)
            .Subscribe(v =>
            {
                weapon.NormalAttackLong(v);
            });
    }
}
