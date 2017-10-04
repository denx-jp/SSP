using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void Init(PlayerModel playerModel);
    void NormalAttack(Animator animator, Vector3 camPos, Vector3 camDir, Quaternion camRot);
}
