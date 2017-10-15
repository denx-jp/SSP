using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

//移動やジャンプにおけるTransform・RigidBodyの操作など
public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] private Rigidbody rigid;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpPower = 6f;
    [SerializeField] private float groundCheckDistance = 1f;

    private bool isOnGround = true;
    private Vector3 groundNormal;
    private float currentGroundCheckDistance;
    private bool isHoldWeapon = false;

    private void Start()
    {
        currentGroundCheckDistance = groundCheckDistance;
    }

    public void Move(Vector3 move, bool isDash)
    {
        if (move.magnitude > 1f) move.Normalize();
        var speed = isDash ? dashSpeed : walkSpeed;
        move = Vector3.ProjectOnPlane(move, groundNormal) * speed;
        rigid.velocity = move;

        if (move != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), Time.deltaTime * rotationSpeed);

        UpdateAnimator(move);
    }

    void UpdateAnimator(Vector3 move)
    {
        var forwardVal = move.magnitude / dashSpeed;
        animator.SetFloat("Move Z", forwardVal, 0.1f, Time.deltaTime);
        animator.SetBool("OnGround", isOnGround);
        if (!isOnGround) animator.SetFloat("Move Y", rigid.velocity.y);
    }

    #region MyRegion
    //public void Jump()
    //{
    //    rigid.velocity = new Vector3(rigid.velocity.x, jumpPower, rigid.velocity.z);
    //    isOnGround = false;
    //    currentGroundCheckDistance = 0.1f;
    //}

    //private void Land()
    //{
    //    isOnGround = true;
    //    currentGroundCheckDistance = groundCheckDistance;
    //}

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
    #endregion
}
