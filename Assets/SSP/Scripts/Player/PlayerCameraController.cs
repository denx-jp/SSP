using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public enum CameraMode { Normal, Battle, Scope }
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private PlayerHealthManager healthManager;
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Transform target;
    [SerializeField] private float correctPosSpeed = 2f;
    [SerializeField] private float yMinLimit = -80;
    [SerializeField] private float yMaxLimit = 80;
    [SerializeField] private float rotationSensitivity = 2.0f;
    [SerializeField] private float battleRotationSensitivity = 2.0f;
    [SerializeField] private float scopeRotationSensitivity = 2.0f;
    [SerializeField] private float battleMagnification = 1.0f;

    [SerializeField] private Vector3 normalModeOffset = new Vector3(0, 0, -3);
    [SerializeField] private Vector3 balltleModeOffset = new Vector3(0.5f, 1.5f, -1.5f);
    [SerializeField] private Vector3 scopeModeOffset = new Vector3(0, 0.1f, 0.3f);

    private CameraMode mode;
    private Vector3 offset;
    private float defaultFieldOfView;
    private Camera mainCamera;
    private Transform cameraTransform;
    private float x, y;

    private void Start()
    {
        var model = GetComponent<PlayerModel>();
        if (!model.isLocalPlayerCharacter) return;

        mode = CameraMode.Normal;
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
        defaultFieldOfView = mainCamera.fieldOfView;

        this.ObserveEveryValueChanged(_ => mode)
            .Subscribe(_ =>
            {
                switch (mode)
                {
                    case CameraMode.Normal:
                        offset = normalModeOffset;
                        break;
                    case CameraMode.Battle:
                        offset = balltleModeOffset;
                        break;
                    case CameraMode.Scope:
                        offset = scopeModeOffset;
                        break;
                }
            });

        // targetとカメラの間に障害物があった時にカメラをtargetに近づける
        this.UpdateAsObservable()
            .Where(_ => mode == CameraMode.Normal)
            .Subscribe(_ =>
            {
                RaycastHit hit;
                var dir = cameraTransform.position - target.position;
                if (Physics.Raycast(target.position, dir, out hit, normalModeOffset.magnitude, LayerMap.StageMask))
                {
                    var distance = Vector3.Distance(hit.point, target.position) * 0.8f;
                    offset = Vector3.Lerp(offset, offset.normalized * distance, Time.deltaTime * correctPosSpeed);
                }
                else
                {
                    offset = Vector3.Lerp(offset, normalModeOffset, Time.deltaTime * correctPosSpeed);
                }
            });

        healthManager.GetDeathStream()
            .Where(v => v)
            .Subscribe(_ =>
            {
                ChangeCameraMode(CameraMode.Normal);
                model.MoveMode = MoveMode.normal;
                LookPlayer();
            });

        pim.CameraRotate
            .Where(v => target != null)
            .Subscribe(input =>
            {
                x += input.x * rotationSensitivity;
                y = ClampAngle(y - input.y * rotationSensitivity, yMinLimit, yMaxLimit);
                LookPlayer();
            });

        pim.CameraResetButtonDown
            .Where(_ => mode == CameraMode.Normal)
            .Where(v => v)
            .Where(_ => target != null)
            .Subscribe(_ =>
            {
                x = transform.eulerAngles.y;
                y = transform.eulerAngles.x;
            });
    }

    public void LookPlayer()
    {
        var rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
        var position = target.position + rotation * offset;

        cameraTransform.position = position;
        cameraTransform.rotation = rotation;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
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

    public void FitNomalModeRotationAndBattleModeRotation()
    {
        // Vector3.upをxだけ回転させるので、y軸の回転を取得
        x = cameraTransform.eulerAngles.y;
    }

    private void SetCameraFov(CameraMode mode)
    {
        var fov = defaultFieldOfView;
        switch (mode)
        {
            case CameraMode.Normal:
                fov = defaultFieldOfView;
                break;
            case CameraMode.Battle:
                fov = defaultFieldOfView / battleMagnification;
                break;
            case CameraMode.Scope:
                var gun = (LongRangeWeaponModel)inventory.weapons[inventory.currentWeaponType].model;
                fov = gun == null ? defaultFieldOfView / battleMagnification : defaultFieldOfView / gun.scopeMagnification;
                break;
        }

        // シーン遷移時にmainCameraが先に破棄されてエラーが出るため
        if (mainCamera != null)
            mainCamera.fieldOfView = fov;
    }
}
