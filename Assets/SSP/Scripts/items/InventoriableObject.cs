using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoriableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInventory.InventoryType inventoryType;
    [SerializeField] private bool canInteract = true;

    public void Interact(PlayerManager pm)
    {
        pm.playerInventory.SetWeapon(this.gameObject, inventoryType);
        canInteract = false;
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public void SetCanInteract(bool _canInteract)
    {
        canInteract = _canInteract;
    }
}
