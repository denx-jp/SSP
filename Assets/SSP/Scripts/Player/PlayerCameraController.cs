using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class PlayerCameraController : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -3);
    private Vector3 temp_offset;
    [SerializeField] private float cameraRotationSpeed = 100;
    [SerializeField] private PlayerInputManager pim;

    private void Start()
    {
        if (!GetComponent<PlayerModel>().isLocalPlayerCharacter) return;

        SetTarget(this.gameObject);
        temp_offset = offset;
        LookPlayer();

        pim.CameraResetButtonDown
            .Where(v => v)
            .Where(_ => target != null)
            .Subscribe(_ =>
            {
                var targetDir = target.transform.forward;
                targetDir = new Vector3(targetDir.x, 0.0f, targetDir.z);
                var rotation = Quaternion.LookRotation(targetDir, Vector3.up);
                temp_offset = rotation * offset;
            });

        pim.CameraRotate
            .Where(v => target != null)
            .Subscribe(input =>
            {
                temp_offset = Quaternion.Euler(0.0f, input.x * Time.deltaTime * cameraRotationSpeed, 0.0f) * temp_offset;

                //ジンバルロックしないように制御
                var temp_delta = target.transform.position - Camere().transform.position;
                if ((Vector3.Dot(temp_delta, new Vector3(temp_delta.x, 0.0f, temp_delta.z))) > 0.1f)
                {
                    temp_offset = Quaternion.AngleAxis(-1.0f * input.y * Time.deltaTime * cameraRotationSpeed, Camere().transform.right) * temp_offset;
                }
                else
                {
                    if (temp_delta.y > 0.0f && input.y < 0.0f)
                        temp_offset = Quaternion.AngleAxis(-1.0f * input.y * Time.deltaTime * cameraRotationSpeed, Camere().transform.right) * temp_offset;
                    else if (temp_delta.y < 0.0f && input.y > 0.0f)
                        temp_offset = Quaternion.AngleAxis(-1.0f * input.y * Time.deltaTime * cameraRotationSpeed, Camere().transform.right) * temp_offset;
                }

                LookPlayer();
            });
    }

    public void SetTarget(GameObject _target)
    {
        target = _target;
    }

    private void LookPlayer()
    {
        Camere().transform.position = target.transform.position + temp_offset;
        var delta = target.transform.position - Camere().transform.position;
        var direction = new Vector3(delta.x, delta.y + offset.y, delta.z);
        Camere().transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private GameObject Camere()
    {
        if (mainCamera != null) return mainCamera;
        mainCamera = GameObject.FindWithTag("MainCamera");
        return mainCamera;
    }

    public void SwitchCamera(bool _isScoped, Camera _scope)
    {
        Camere().SetActive(_isScoped);
        _scope.gameObject.SetActive(!_isScoped);
    }
}
