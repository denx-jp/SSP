﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void Init(PlayerModel playerModel);
    void NormalAttack();
}
