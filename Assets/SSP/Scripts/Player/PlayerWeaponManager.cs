using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerWeaponManager : NetworkBehaviour
{
    private PlayerInputManager pim;
    public IAttackable attacker;

    void Start()
    {
        pim = GetComponent<PlayerInputManager>();

        pim.AttackButtonShort
            .Where(input => input)
            .Where(_ => attacker != null)
            .Subscribe(_ =>
            {
                attacker.NormalAttack();
            });
    }
}
