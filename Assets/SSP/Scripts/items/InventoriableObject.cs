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
        SetTransformToOwner();
    }

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

    [ClientCallback]
    private void SetTransformToOwner()
    {
        var player = ClientScene.FindLocalObject(ownerPlayerId);
        if (player == null) return;
        var pim = player.GetComponent<PlayerInventoryManager>();
        pim.SetDefaultWeapon(this.gameObject, inventoriableType);
    }

    public void SetTransformOwnerHand(Transform leftHand, Transform rightHand)
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
