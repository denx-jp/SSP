using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerAnimationController : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerLocomotor locomotor;
    [SerializeField] private PlayerController controller;

    [SerializeField] private PlayerHealthManager healthManager;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private PlayerAvoider avoider;

    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                animator.SetBool("OnGround", locomotor.isGrounded);

                var x = controller.mode == MoveMode.battle ? Vector3.Dot(transform.right, rb.velocity) : 0;
                animator.SetFloat("Move X", x);
                animator.SetFloat("Move Y", rb.velocity.y);
                animator.SetFloat("Move Z", Vector3.Dot(transform.forward, rb.velocity));
                animator.SetBool("Battle Mode", controller.mode == MoveMode.battle);
            });

        healthManager.GetDeathStream()
            .Where(_ => isLocalPlayer)
            .Subscribe(isdeath =>
            {
                CmdStartDeathAnimation(isdeath);
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
        animator.SetTrigger("Attack");
    }

    #region Death
    [Command]
    private void CmdStartDeathAnimation(bool isdeath)
    {
        RpcStartDeathAnimation(isdeath);
    }

    [ClientRpc]
    private void RpcStartDeathAnimation(bool isdeath)
    {
        animator.SetBool("Death", isdeath);
    }
    #endregion
}
