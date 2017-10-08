using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoriableType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick }
public class InventoriableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoriableType inventoriableType;
    [SerializeField] private bool canInteract = true;
    [SerializeField] public Vector3 weaponPos;
    [SerializeField] public Vector3 weaponRotate;

    [SerializeField] public enum Hands { leftHand, rightHand };

    public void Interact(PlayerManager pm)
    {
        pm.playerInventoryManager.SetWeaponToInventory(this.gameObject, inventoriableType);
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

    public void HoldHand(GameObject leftHand,GameObject rightHand){
        
        Hands hand = Hands.leftHand;
        //Hands hand = Hands.rightHand;

        switch(hand){
            case Hands.leftHand:
                transform.parent = leftHand.transform;
                break;
            case Hands.rightHand:
                transform.parent = rightHand.transform;
                break;
            default:
                break;
        }
    }
}
