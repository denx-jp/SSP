using UnityEngine;

public class InteractableGuide : GuideObject
{
    private IInteractable interactable;

    private void Start()
    {
        interactable = GetComponentInParent<IInteractable>();
    }

    public override bool ShouldGuide()
    {
        return interactable.CanInteract();
    }
}
