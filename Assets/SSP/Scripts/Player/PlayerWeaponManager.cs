using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerWeaponManager : NetworkBehaviour
{
    private PlayerInputManager pim;
    public IAttackable attacker;

    void Start()
    {
        pim = GetComponent<PlayerInputManager>();

        this.UpdateAsObservable()
            .SkipUntil(pim.AttackButtonLong.Where(t => t))
            .TakeUntil(pim.AttackButtonLong.Where(f => !f))
            .Where(_ => attacker != null)
            .RepeatUntilDestroy(gameObject)
            .Subscribe(_ =>
            {
                attacker.NormalAttack();
            });

        pim.ScopeButtonLong
            .Where(_ => attacker != null)
            .Subscribe(_ =>
            {
                attacker.LongPressScope();
            });
    }
}
