using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

//移動やジャンプにおけるTransform・RigidBodyの操作など
public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] private Rigidbody rigid;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpPower = 6f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float groundCheckDistance = 1f;

    [SerializeField] public bool isOnGround = true;
    private Vector3 groundNormal;
    [SerializeField] private float currentGroundCheckDistance;
    private bool isHoldWeapon = false;

    private void Start()
    {
        currentGroundCheckDistance = groundCheckDistance;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                CheckGroundStatus();

                if (isOnGround)
                    Debug.Log("a");
                else
                {
                    //重力が弱いので
                    Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
                    rigid.AddForce(extraGravityForce);
                    currentGroundCheckDistance = rigid.velocity.y < 0 ? groundCheckDistance : 0.01f;
                }
            });
    }

    public void Move(Vector3 move, bool isDash)
    {
        if (move.magnitude > 1f) move.Normalize();
        var speed = isDash ? dashSpeed : walkSpeed;
        move = Vector3.ProjectOnPlane(move, groundNormal) * speed;

        if (move != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), Time.deltaTime * rotationSpeed);

        move.y = rigid.velocity.y;
        rigid.velocity = move;

        UpdateAnimator(move);
    }

    void UpdateAnimator(Vector3 move)
    {
        var forwardVal = move.magnitude / dashSpeed;
        animator.SetFloat("Move Z", forwardVal, 0.1f, Time.deltaTime);
        animator.SetBool("OnGround", isOnGround);
        if (!isOnGround) animator.SetFloat("Move Y", rigid.velocity.y);
    }

    public void Jump()
    {
        rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        isOnGround = false;
        currentGroundCheckDistance = 0.01f;
    }

    private void CheckGroundStatus()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hitInfo, currentGroundCheckDistance))
        {
            isOnGround = true;
            groundNormal = hitInfo.normal;
        }
        else
        {
            isOnGround = false;
            groundNormal = Vector3.up;
        }
    }
}
