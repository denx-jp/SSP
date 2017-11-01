using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void Init(PlayerModel playerModel);
    void NormalAttack();
    void NormalAttackLong(bool active);
    void LongPressScope(bool input);
}
