using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void Init(PlayerModel pm);
    void NormalAttack();
}
