using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class CarriableLSS : CarriableObject
{
    private LifeSupportSystemModel model;

    protected override void Start()
    {
        model = GetComponent<LifeSupportSystemModel>();
        base.Start();
    }

    public override bool CanCarry()
    {
        // CarriableObjectの判定はLocalPlayerでのみ受け付けるのでLocalPlayerのTeamIdから判断
        return base.CanCarry() && ClientPlayersManager.Instance.GetLocalPlayer().playerModel.teamId == model.teamId;
    }
}
