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

    public override bool CanCarry(int teamId)
    {
        return teamId == model.teamId;
    }
}
