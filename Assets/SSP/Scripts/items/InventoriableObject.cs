using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum InventoriableType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick }
public class InventoriableObject : NetworkBehaviour, IInteractable
{
    [SerializeField] public InventoriableType inventoriableType;
    [SerializeField] private bool canInteract = true;
    [SerializeField] public Vector3 weaponPos;
    [SerializeField] public Vector3 weaponRot;

    private enum Hands { leftHand, rightHand };
    [SerializeField] private Hands hand;

    [SyncVar] public NetworkInstanceId ownerPlayerId;

    void Start()
    {
        DefaultWeaponSetup();
    }

    public void Interact(PlayerManager pm)
    {
        SetTransformOwnerHand(pm.playerInventoryManager.leftHandTransform, pm.playerInventoryManager.rightHandTransform);
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

    //デフォルト装備を持ち主のインベントリに格納・装備する
    [ClientCallback]
    private void DefaultWeaponSetup()
    {
        if (ownerPlayerId == null) return;
        var owner = ClientScene.FindLocalObject(ownerPlayerId);
        if (owner == null) return;
        var pim = owner.GetComponent<PlayerInventoryManager>();

        pim.SetWeaponToInventory(this.gameObject, inventoriableType);
        var inventoryType = pim.ConvertInventoriableTypeToInventoryType(inventoriableType);
        pim.inventory.EquipWeapon(inventoryType);
        SetTransformOwnerHand(pim.leftHandTransform, pim.rightHandTransform);
    }

    private void SetTransformOwnerHand(Transform leftHand, Transform rightHand)
    {
        switch (hand)
        {
            case Hands.leftHand:
                transform.parent = leftHand;
                break;
            case Hands.rightHand:
                transform.parent = rightHand;
                break;
        }

        transform.localPosition = weaponPos;
        transform.localRotation = Quaternion.Euler(weaponRot.x, weaponRot.y, weaponRot.z);
    }
}
