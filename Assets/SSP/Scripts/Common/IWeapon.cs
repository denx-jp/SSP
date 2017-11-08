using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void Init(PlayerManager playerManager);
    void NormalAttack();
    void NormalAttackLong(bool active);
    void LongPressScope(bool active);
    void SwitchScope();
}
