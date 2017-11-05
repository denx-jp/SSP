using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private NetworkAnimator networkAnimator;
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
           .Subscribe(_ => state = networkAnimator.animator.GetCurrentAnimatorStateInfo(0));

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                networkAnimator.animator.SetBool("OnGround", locomotor.isGrounded);

                var x = model.MoveMode == MoveMode.battle ? Vector3.Dot(transform.right, rb.velocity) : 0;
                networkAnimator.animator.SetFloat("Move X", x);
                networkAnimator.animator.SetFloat("Move Y", rb.velocity.y);
                networkAnimator.animator.SetFloat("Move Z", Vector3.Dot(transform.forward, rb.velocity));
                networkAnimator.animator.SetBool("Battle Mode", model.MoveMode == MoveMode.battle);
            });

        healthManager.GetDeathStream()
            .Subscribe(isdeath =>
            {
                var trigger = isdeath ? "Death" : "Revive";
                networkAnimator.SetTrigger(trigger);
            });

        this.ObserveEveryValueChanged(_ => inventory.currentWeaponType)
            .Subscribe(type =>
            {
                networkAnimator.animator.SetInteger("Weapon", (int)type);
                networkAnimator.SetTrigger("Sheath");
            });
    }

    public void Attack()
    {
        if (model.MoveMode != MoveMode.battle) return;
        networkAnimator.SetTrigger("Attack");
    }

    public void Avoid()
    {
        if (model.MoveMode != MoveMode.battle) return;
        networkAnimator.SetTrigger("Avoid");
    }

    public void Active()
    {
        networkAnimator.SetTrigger("Active");
    }

    public void Pickup()
    {
        networkAnimator.SetTrigger("Pickup");
    }
}
