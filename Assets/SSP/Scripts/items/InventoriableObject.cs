using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoriableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInventory.InventoryType inventoryType;

    public void Interact(PlayerManager pm)
    {
        switch (inventoryType)
        {
            case PlayerInventory.InventoryType.LongRangeWeapon:
                pm.palyerInventory.SetLongRangeWeapon(this.gameObject);
                break;
            case PlayerInventory.InventoryType.ShortRangeWeapon:
                pm.palyerInventory.SetShortRangeWeapon(this.gameObject);
                break;
            case PlayerInventory.InventoryType.Gimmick:
                pm.palyerInventory.AddGimmick(this.gameObject);
                break;
        }
    }
}
