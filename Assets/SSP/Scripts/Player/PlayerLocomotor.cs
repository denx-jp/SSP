using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerLocomotor : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    private Collider col;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotateSpeed = 12f;
    [SerializeField] private float jumpSpeed = 6f;
    [SerializeField] private float gravityMultiplier = -9.8f;
    [SerializeField] private float maxVelocity = 2f;
    [SerializeField] private float minVelocity = -2f;
    [SerializeField] private float maxAngle = 90f;

    // 接地判定
    [SerializeField] float groundCheckDistance = 1f;
    [SerializeField] Vector3 boxCastoffset;
    [SerializeField] Vector3 boxCastExtents;
    private RaycastHit hit;

    // 摩擦
    [SerializeField] private float groundDynamicFriction = 0.6f;
    [SerializeField] private float groundStaticFriction = 0.6f;

    public bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        col = GetComponents<Collider>().First(v => !v.isTrigger);

        this.ObserveEveryValueChanged(_ => isGrounded)
            .Subscribe(_ =>
            {
                if(isGrounded)
                {
                    col.material.dynamicFriction = groundDynamicFriction;
                    col.material.staticFriction = groundStaticFriction;
                }
                else
                {
                    col.material.dynamicFriction = 0;
                    col.material.staticFriction = 0;
                }
            });
    }

    private void FixedUpdate()
    {
        // 重力をかける
        rb.AddForce(0, gravityMultiplier, 0, ForceMode.Acceleration);

        CheckForGrounded();

        if (!isGrounded)
            AirControl();
    }

    void CheckForGrounded()
    {
        var isHit = Physics.BoxCast(transform.position + boxCastoffset, boxCastExtents / 2, Vector3.down, out hit, transform.rotation, groundCheckDistance);
        if (isHit)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    #region 移動(地上)
    public void Move(Vector3 input, bool isRun, MoveMode mode)
    {
        Vector3 moveDir = input.magnitude > 1 ? input.normalized : input;
        Vector3 newVelocity;
        if (mode == MoveMode.normal && isRun)
            newVelocity = moveDir * runSpeed;
        else if (!isGrounded)
            newVelocity = moveDir * walkSpeed * 0.5f;
        else
            newVelocity = moveDir * walkSpeed;

        // 落下中ならy速度はそのまま
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
    }

    public void RotateTowardsMovementDir(Vector3 input)
    {
        if (input == Vector3.zero || !isGrounded) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(input), Time.deltaTime * rotateSpeed);
    }

    public void RotateCameraDir(Vector3 camerDir)
    {
        Vector3 localCameraDirection = transform.InverseTransformDirection(camerDir);
        float angle = Mathf.Atan2(localCameraDirection.x, localCameraDirection.z) * Mathf.Rad2Deg;

        // Find the rotation
        float rotation = angle * Time.deltaTime * rotateSpeed;

        // Clamp the rotation to maxAngle
        if (angle > maxAngle) rotation = Mathf.Clamp(rotation, angle - maxAngle, rotation);
        if (angle < -maxAngle) rotation = Mathf.Clamp(rotation, rotation, angle + maxAngle);

        // Rotate the character
        transform.Rotate(Vector3.up, rotation);
    }
    #endregion

    #region ジャンプ
    public void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            animator.SetBool("OnGround", isGrounded);
            rb.velocity += jumpSpeed * Vector3.up;
        }
    }

    void AirControl()
    {
        // 達成可能な速度を制限する
        float velocityX = 0;
        float velocityZ = 0;
        if (rb.velocity.x > maxVelocity)
        {
            velocityX = rb.velocity.x - maxVelocity;
            if (velocityX < 0) velocityX = 0;
            rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
        }
        if (rb.velocity.x < minVelocity)
        {
            velocityX = rb.velocity.x - minVelocity;
            if (velocityX > 0) velocityX = 0;
            rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
        }
        if (rb.velocity.z > maxVelocity)
        {
            velocityZ = rb.velocity.z - maxVelocity;
            if (velocityZ < 0) velocityZ = 0;
            rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
        }
        if (rb.velocity.z < minVelocity)
        {
            velocityZ = rb.velocity.z - minVelocity;
            if (velocityZ > 0) velocityZ = 0;
            rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
        }
    }
    #endregion
}
