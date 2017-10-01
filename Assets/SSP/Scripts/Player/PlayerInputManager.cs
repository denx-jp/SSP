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
    public IObservable<float> WeaponChange { get; private set; }
    public readonly Subject<float> WeaponChangeWhellScroll = new Subject<float>();
    public readonly Subject<bool> WeaponChangeButtonDown = new Subject<bool>();

    private Vector2 mouseInput;
    private Vector2 gamePadInput;
    private Vector2 moveInput;

    private PlayerModel playerModel;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        var convertFloatStream = WeaponChangeButtonDown.Where(v => v).Select(v => 0.1f);
        WeaponChange = Observable.Merge(WeaponChangeWhellScroll, convertFloatStream).Where(v => v != 0);

        if(playerModel.isLocalPlayerCharacter)
        {

        this.UpdateAsObservable()
            .Where(_ => playerModel.IsAlive())
            .Subscribe(_ =>
            {
                mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                CameraRotate.OnNext(mouseInput);
                gamePadInput = new Vector2(Input.GetAxis("GamePadRightStick X"), Input.GetAxis("GamePadRightStick Y"));
                CameraRotate.OnNext(gamePadInput);
                CameraResetButtonDown.OnNext(Input.GetButtonDown("Camera Reset"));

                moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                Move.OnNext(moveInput);

                AvoidButtonDown.OnNext(Input.GetButtonDown("Avoid"));
                DashButtonDown.OnNext(Input.GetButton("Dash"));
                JumpButtonDown.OnNext(Input.GetButtonDown("Jump"));

                NormalAttackButtonDown.OnNext(Input.GetButtonDown("Normal Attack"));
                ActionButtonDown.OnNext(Input.GetButtonDown("Action"));

                WeaponChangeWhellScroll.OnNext(Input.GetAxis("Mouse ScrollWheel"));
                WeaponChangeButtonDown.OnNext(Input.GetButtonDown("Weapon Change"));
            });

        this.UpdateAsObservable()
            .Where(_ => !playerModel.IsAlive())
            .Subscribe(_ =>
            {
                //死亡時入力受付ストリーム
            });

        }
    }

}
