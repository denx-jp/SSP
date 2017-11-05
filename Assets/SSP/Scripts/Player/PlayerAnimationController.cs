﻿using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerModel model;
    [SerializeField] private PlayerLocomotor locomotor;

    [SerializeField] private PlayerHealthManager healthManager;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerAvoider avoider;

    private AnimatorStateInfo state;

    private void Start()
    {
        if (!model.isLocalPlayerCharacter) return;
        this.UpdateAsObservable()
           .Subscribe(_ => state = animator.GetCurrentAnimatorStateInfo(0));

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                animator.SetBool("OnGround", locomotor.isGrounded);

                var x = model.MoveMode == MoveMode.battle ? Vector3.Dot(transform.right, rb.velocity) : 0;
                animator.SetFloat("Move X", x);
                animator.SetFloat("Move Y", rb.velocity.y);
                animator.SetFloat("Move Z", Vector3.Dot(transform.forward, rb.velocity));
                animator.SetBool("Battle Mode", model.MoveMode == MoveMode.battle);
            });

        healthManager.GetDeathStream()
            .Subscribe(isdeath =>
            {
                var trigger = isdeath ? "Death" : "Revive";
                animator.SetTrigger(trigger);
            });

        this.ObserveEveryValueChanged(_ => inventory.currentWeaponType)
            .Subscribe(type =>
            {
                animator.SetInteger("Weapon", (int)type);
                animator.SetTrigger("Sheath");
            });
    }

    public void Attack()
    {
        if (model.MoveMode != MoveMode.battle) return;
        animator.SetTrigger("Attack");
    }

    public void Avoid()
    {
        if (model.MoveMode != MoveMode.battle) return;
        animator.SetTrigger("Avoid");
    }

    public void Active()
    {
        animator.SetTrigger("Active");
    }

    public void Pickup()
    {
        animator.SetTrigger("Pickup");
    }
}
