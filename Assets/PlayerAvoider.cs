using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerAvoider : MonoBehaviour
{

    private Animator animator;
    [SerializeField] private WeaponAttacker weaponAttacker;
    private PlayerInputManager pim;

    void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();

        pim.AvoidButtonDown
            .Where(v => v)
            .Subscribe(v =>
            {
                animator.SetTrigger("RollForward");
            });

    }
}
