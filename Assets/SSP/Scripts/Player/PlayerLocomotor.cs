using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerLocomotor : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;

    [SerializeField] protected float walkSpeed = 3f;
    [SerializeField] protected float runSpeed = 6f;
    [SerializeField] protected float rotateSpeed = 10f;
    [SerializeField] private float jumpSpeed = 6f;
    [SerializeField] private float gravityMultiplier = -9.8f;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float inAirSpeed = 8f;
    [SerializeField] private float maxVelocity = 2f;
    [SerializeField] private float minVelocity = -2f;
    [SerializeField] private float maxAngle = 90f;

    public bool isGrounded;
    private bool isJumping;
    private bool isFalling;

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
        RaycastHit hit;
        Vector3 rayOffset = Vector3.up * 0.1f;
        if (Physics.Raycast((transform.position + rayOffset), -Vector3.up, out hit, groundCheckDistance))
        {
            isGrounded = true;
            isFalling = false;
            //if (!isJumping)
            //    animator.SetInteger("Jumping", 0);
        }
        else
        {
            isGrounded = false;
        }
    }

    #region 移動(地上)
    public void Move(Vector3 input, bool isRun)
    {
        if (!isGrounded) return;
        Vector3 motion = input.magnitude > 1 ? input.normalized : input;
        Vector3 newVelocity = isRun ? motion * runSpeed : motion * walkSpeed;

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
        Debug.Log(rotation.ToString("0.00"));
    }
    #endregion

    #region ジャンプ
    public void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            isJumping = true;
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
