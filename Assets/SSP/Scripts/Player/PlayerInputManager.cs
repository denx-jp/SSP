using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class PlayerInputManager : MonoBehaviour
{

    public readonly Subject<Vector2> CameraRotate = new Subject<Vector2>();
    public readonly Subject<bool> CameraResetButtonDown = new Subject<bool>();

    public readonly Subject<Vector2> Move = new Subject<Vector2>();
    public readonly Subject<bool> AvoidButtonDown = new Subject<bool>();
    public readonly Subject<bool> DashButtonDown = new Subject<bool>();
    public readonly Subject<bool> JumpButtonDown = new Subject<bool>();

    public readonly Subject<bool> NormalAttackButtonDown = new Subject<bool>();
    public readonly Subject<bool> NormalAttackButtonUp = new Subject<bool>();
    public readonly Subject<bool> NormalAttackButtonLong = new Subject<bool>();

    public readonly Subject<bool> ScopeButtonDown = new Subject<bool>();
    public readonly Subject<bool> ScopeButtonUp = new Subject<bool>();
    public readonly Subject<bool> ScopeButtonLong = new Subject<bool>();

    public readonly Subject<bool> ActionButtonDown = new Subject<bool>();

    private Vector2 mouseInput;
    private Vector2 gamePadInput;
    private Vector2 moveInput;

    private PlayerModel playerModel;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();

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
                NormalAttackButtonUp.OnNext(Input.GetButtonUp("Normal Attack"));
                ScopeButtonDown.OnNext(Input.GetButtonDown("Scope"));
                ScopeButtonUp.OnNext(Input.GetButtonUp("Scope"));
                ActionButtonDown.OnNext(Input.GetButtonDown("Action"));
            });

        NormalAttackButtonDown.Where(x => x).Do(_=> Debug.Log("AttackSigleClick")).Delay(TimeSpan.FromMilliseconds(500)).TakeUntil(NormalAttackButtonUp.Where(y => y))
            .RepeatUntilDestroy(gameObject).Subscribe(_ => { Debug.Log("AttackLongClick"); NormalAttackButtonLong.OnNext(true); });
        NormalAttackButtonUp.Where(x => x).Subscribe(_ => NormalAttackButtonLong.OnNext(false));

        ScopeButtonDown.Where(x => x).Do(_=> Debug.Log("AttackSigleClick")).Delay(TimeSpan.FromMilliseconds(500)).TakeUntil(ScopeButtonUp.Where(y => y))
            .RepeatUntilDestroy(gameObject).Subscribe(_ => { Debug.Log("ScopeLong"); ScopeButtonLong.OnNext(true); });
        ScopeButtonUp.Where(x => x).Subscribe(_ => ScopeButtonLong.OnNext(false));

        this.UpdateAsObservable()
            .Where(_ => !playerModel.IsAlive())
            .Subscribe(_ =>
            {
                //死亡時入力受付ストリーム
            });
    }

}
