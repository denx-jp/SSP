using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private float longPressSecond;

    public readonly Subject<Vector2> CameraRotate = new Subject<Vector2>();
    public readonly Subject<bool> CameraResetButtonDown = new Subject<bool>();

    public readonly Subject<Vector2> Move = new Subject<Vector2>();
    public readonly Subject<bool> AvoidButtonDown = new Subject<bool>();
    public readonly Subject<bool> DashButtonDown = new Subject<bool>();
    public readonly Subject<bool> JumpButtonDown = new Subject<bool>();

    #region Mouse Button
    public readonly Subject<bool> AttackButtonDown = new Subject<bool>();
    public readonly Subject<bool> AttackButtonUp = new Subject<bool>();
    public readonly Subject<bool> AttackButtonShort = new Subject<bool>();
    public readonly Subject<bool> AttackButtonLong = new Subject<bool>();
    public readonly Subject<bool> ScopeButtonDown = new Subject<bool>();
    public readonly Subject<bool> ScopeButtonUp = new Subject<bool>();
    public readonly Subject<bool> ScopeButtonShort = new Subject<bool>();
    public readonly Subject<bool> ScopeButtonLong = new Subject<bool>();
    public readonly Subject<bool> ActionButtonDown = new Subject<bool>();
    #endregion

    public UniRx.IObservable<float> WeaponChange { get; private set; }
    public readonly Subject<float> WeaponChangeWhellScroll = new Subject<float>();
    public readonly Subject<bool> WeaponChangeButtonDown = new Subject<bool>();

    private Vector2 mouseInput;
    private Vector2 gamePadInput;
    private Vector2 moveInput;

    private void Start()
    {
        var playerModel = GetComponent<PlayerModel>();
        var convertFloatStream = WeaponChangeButtonDown.Where(v => v).Select(v => 0.1f);
        WeaponChange = Observable.Merge(WeaponChangeWhellScroll, convertFloatStream).Where(v => v != 0);

        if (!playerModel.isLocalPlayerCharacter) return;

        PrepareMouseClickInput(AttackButtonDown, AttackButtonUp, AttackButtonShort, AttackButtonLong);
        PrepareMouseClickInput(ScopeButtonDown, ScopeButtonUp, ScopeButtonShort, ScopeButtonLong);

        this.UpdateAsObservable()
            .Where(_ => playerModel.IsAlive())
            .Where(_ => GameManager.IsGameStarting())
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

                AttackButtonDown.OnNext(Input.GetButtonDown("Normal Attack"));
                AttackButtonUp.OnNext(Input.GetButtonUp("Normal Attack"));
                ScopeButtonDown.OnNext(Input.GetButtonDown("Scope"));
                ScopeButtonUp.OnNext(Input.GetButtonUp("Scope"));
                ActionButtonDown.OnNext(Input.GetButtonDown("Action"));

                WeaponChangeWhellScroll.OnNext(Input.GetAxis("Mouse ScrollWheel"));
                WeaponChangeButtonDown.OnNext(Input.GetButtonDown("Weapon Change"));
            });

        this.UpdateAsObservable()
            .Where(_ => !playerModel.IsAlive())
            .Where(_ => GameManager.IsGameStarting())
            .Subscribe(_ =>
            {
                //死亡時入力受付ストリーム
            });

    }

    void PrepareMouseClickInput(Subject<bool> buttonDown, Subject<bool> buttonUp, Subject<bool> buttonShort, Subject<bool> buttonLong)
    {
        //長押し開始判定
        buttonDown.Where(x => x)
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(longPressSecond)))
            .TakeUntil(buttonUp.Where(x => x))
            .RepeatUntilDestroy(this.gameObject)
            .Subscribe(_ => buttonLong.OnNext(true));

        //長押し終了判定
        buttonUp
            .SkipUntil(buttonLong.Where(v => v))
            .Where(x => x)
            .Take(1)
            .RepeatUntilDestroy(gameObject)
            .Subscribe(_ => buttonLong.OnNext(false));

        //クリック判定
        buttonDown.Where(x => x)
            .Timestamp()
            .Zip(buttonUp.Where(x => x).Timestamp(), (d, u) => (u.Timestamp - d.Timestamp).TotalMilliseconds / 1000.0f)
            .Where(time => time < longPressSecond)
            .Subscribe(t => buttonShort.OnNext(true));
    }
}
