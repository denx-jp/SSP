using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerInputManager : MonoBehaviour
{

    public readonly Subject<Vector2> CameraRotate = new Subject<Vector2>();
    public readonly Subject<bool> CameraResetButtonDown = new Subject<bool>();

    public readonly Subject<Vector2> Move = new Subject<Vector2>();
    public readonly Subject<bool> AvoidButtonDown = new Subject<bool>();
    public readonly Subject<bool> DashButtonDown = new Subject<bool>();
    public readonly Subject<bool> JumpButtonDown = new Subject<bool>();

    public readonly Subject<bool> NormalAttackButtonDown = new Subject<bool>();
    public readonly Subject<bool> ActionButtonDown = new Subject<bool>();

    private Vector2 mouseInput;
    private Vector2 gamePadInput;
    private Vector2 moveInput;

    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                CameraRotate.OnNext(mouseInput);
                gamePadInput = new Vector2(Input.GetAxis("GamePadRightStick X"), Input.GetAxis("GamePadRightStick Y"));
                Debug.Log(gamePadInput);
                CameraRotate.OnNext(gamePadInput);
                CameraResetButtonDown.OnNext(Input.GetButtonDown("Camera Reset"));

                moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                Move.OnNext(moveInput);

                AvoidButtonDown.OnNext(Input.GetButtonDown("Avoid"));
                DashButtonDown.OnNext(Input.GetButton("Dash"));
                JumpButtonDown.OnNext(Input.GetButtonDown("Jump"));

                NormalAttackButtonDown.OnNext(Input.GetButtonDown("Normal Attack"));
                ActionButtonDown.OnNext(Input.GetButtonDown("Action"));
            });
    }

}
