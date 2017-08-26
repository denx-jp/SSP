using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttacker : MonoBehaviour
{
    public void NormalAttack(Animator animator)
    {
        animator.SetTrigger("Attack");
    }

}
