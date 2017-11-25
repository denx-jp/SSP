using UnityEngine;

public class CarriableGuide : GuideObject
{
    private CarriableObject carriable;

    private void Start()
    {
        carriable = GetComponentInParent<CarriableObject>();
    }

    public override bool ShouldGuide()
    {
        return carriable.CanCarry();
    }
}
