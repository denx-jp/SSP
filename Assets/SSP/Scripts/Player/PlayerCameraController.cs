using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public enum CameraMode { Normal, Battle, Scope }
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private Transform target;
    [SerializeField] private PlayerInventory inventory;
    private CameraMode mode;
    private float defaultFieldOfDistance;

    [Header("Noraml Mode")]
    [SerializeField]
    private float cameraRotationSpeed = 100;
    [SerializeField] private Vector3 normalModeOffset = new Vector3(0, 0, -3);
    private Transform cameraTransform;
    private Vector3 tempOffset;

    [Header("Battle Mode")]
    [SerializeField]
    private float yMinLimit = -80; // Min vertical angle
    [SerializeField] private float yMaxLimit = 80; // Max vertical angle
    [SerializeField] private float rotationSensitivity = 3.5f; // The sensitivity of rotation
    [SerializeField] private Vector3 balltleModeOffset = new Vector3(0.5f, 1.5f, -1.5f); // The offset from target relative to camera rotation
    private float x, y;
    private float battleMagnification = 1.0f;

    [Header("Scope Mode")]
    [SerializeField]
    private float scopeRotationSensitivity = 3.5f;
    [HideInInspector] private Vector3 scopeModeOffset = new Vector3(0, 0.1f, 0.3f);

    private void Start()
    {
        if (!GetComponent<PlayerModel>().isLocalPlayerCharacter) return;

        cameraTransform = Camera.main.transform;
        mode = CameraMode.Normal;
        defaultFieldOfDistance = Camera.main.fieldOfView;

        #region Normal Mode
        tempOffset = normalModeOffset;
        LookPlayer();

        pim.CameraResetButtonDown
            .Where(_ => mode == CameraMode.Normal)
            .Where(v => v)
            .Where(_ => target != null)
            .Subscribe(_ =>
            {
                var targetDir = target.transform.forward;
                targetDir = new Vector3(targetDir.x, 0.0f, targetDir.z);
                var rotation = Quaternion.LookRotation(targetDir, Vector3.up);
                tempOffset = rotation * normalModeOffset;
            });

        pim.CameraRotate
            .Where(_ => mode == CameraMode.Normal)
            .Where(v => target != null)
            .Subscribe(input =>
            {
                tempOffset = Quaternion.Euler(0.0f, input.x * Time.deltaTime * cameraRotationSpeed, 0.0f) * tempOffset;

                //ジンバルロックしないように制御
                var temp_delta = target.transform.position - cameraTransform.position;
                if ((Vector3.Dot(temp_delta, new Vector3(temp_delta.x, 0.0f, temp_delta.z))) > 0.1f)
                {
                    tempOffset = Quaternion.AngleAxis(-1.0f * input.y * Time.deltaTime * cameraRotationSpeed, cameraTransform.right) * tempOffset;
                }
                else
                {
                    if (temp_delta.y > 0.0f && input.y < 0.0f)
                        tempOffset = Quaternion.AngleAxis(-1.0f * input.y * Time.deltaTime * cameraRotationSpeed, cameraTransform.right) * tempOffset;
                    else if (temp_delta.y < 0.0f && input.y > 0.0f)
                        tempOffset = Quaternion.AngleAxis(-1.0f * input.y * Time.deltaTime * cameraRotationSpeed, cameraTransform.right) * tempOffset;
                }

                LookPlayer();
            });
        #endregion

        #region Battle/Scope Mode
        pim.CameraRotate
            .Where(_ => mode != CameraMode.Normal)
            .Where(_ => target != null)
            .Subscribe(input =>
            {
                x += input.x * rotationSensitivity;
                y = ClampAngle(y - input.y * rotationSensitivity, yMinLimit, yMaxLimit);
                var offset = mode == CameraMode.Battle ? balltleModeOffset : scopeModeOffset;

                var rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
                var position = target.position + rotation * offset;

                cameraTransform.position = position;
                cameraTransform.rotation = rotation;
            });
        #endregion
    }

    private void LookPlayer()
    {
        cameraTransform.position = target.transform.position + tempOffset;
        var delta = target.transform.position - cameraTransform.position;
        var direction = new Vector3(delta.x, delta.y + normalModeOffset.y, delta.z);
        cameraTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void ChangeCameraMode(CameraMode _mode)
    {
        mode = _mode;
        SetCameraFov(mode);
    }

    public void SetScopeOffset(Vector3 offset)
    {
        scopeModeOffset = offset;
    }

    public void FitRotate()
    {
        // Vector3.upをxだけ回転させるので、y軸の回転を取得
        x = cameraTransform.eulerAngles.y;
    }

    private void SetCameraFov(CameraMode mode)
    {
        var fov = defaultFieldOfDistance;
        switch (mode)
        {
            case CameraMode.Normal:
                fov = defaultFieldOfDistance;
                break;
            case CameraMode.Battle:
                fov = defaultFieldOfDistance / battleMagnification;
                break;
            case CameraMode.Scope:
                var gun = inventory.weapons[inventory.currentWeaponType].model;
                if (gun == null) fov = defaultFieldOfDistance / battleMagnification;
                fov = defaultFieldOfDistance / gun.scopeMagnification;
                break;
        }

        Camera.main.fieldOfView = fov;
    }
}
