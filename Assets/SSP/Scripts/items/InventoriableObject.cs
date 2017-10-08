using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoriableType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick }
public class InventoriableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoriableType inventoriableType;
    [SerializeField] private bool canInteract = true;
    [SerializeField] public Vector3 weaponPos;
    [SerializeField] public Vector3 weaponRot;

    private enum Hands { leftHand, rightHand };
    [SerializeField] private Hands hand;

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

    public void SetEquipWeapon(GameObject leftHand,GameObject rightHand){
        transform.localPosition = weaponPos;
        transform.localRotation = Quaternion.Euler(weaponRot.x, weaponRot.y, weaponRot.z);

        switch(hand){
            case Hands.leftHand:
                transform.parent = leftHand.transform;
                break;
            case Hands.rightHand:
                transform.parent = rightHand.transform;
                break;
        }
    }
}
