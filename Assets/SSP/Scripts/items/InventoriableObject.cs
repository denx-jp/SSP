using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InventoriableObject : NetworkBehaviour, IInteractable
{
    [SerializeField] public WeaponModel model;
    [SerializeField] private bool canInteract = true;
    [SerializeField] public Vector3 weaponPos;
    [SerializeField] public Vector3 weaponRot;

    private enum Hands { leftHand, rightHand };
    [SerializeField] private Hands hand;

    [SyncVar] public NetworkInstanceId ownerPlayerId;
    private NetworkIdentity networkIdentity;

    void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        DefaultWeaponSetup();
    }

    [Server]
    public void Interact(PlayerManager pm)
    {
        RpcSetToInventory(pm.gameObject);
        networkIdentity.AssignClientAuthority(pm.connectionToClient);
    }

    [ClientRpc]
    void RpcSetToInventory(GameObject player)
    {
        canInteract = false;
        ownerPlayerId = player.GetComponent<NetworkIdentity>().netId;
        var pm = player.GetComponent<PlayerManager>();
        SetTransformOwnerHand(pm.playerInventoryManager.leftHandTransform, pm.playerInventoryManager.rightHandTransform);
        pm.playerInventoryManager.SetWeaponToInventory(this.gameObject, model.type);
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public void SetCanInteract(bool _canInteract)
    {
        canInteract = _canInteract;
    }

    //所持中の武器を持ち主のインベントリに格納・装備する
    [ClientCallback]
    private void DefaultWeaponSetup()
    {
        var owner = ClientScene.FindLocalObject(ownerPlayerId);
        if (owner == null) return;
        var pim = owner.GetComponent<PlayerInventoryManager>();

        pim.SetWeaponToInventory(this.gameObject, model.type);
        var inventoryType = pim.ConvertInventoriableTypeToInventoryType(model.type);
        if (pim.inventory.currentWeaponType == inventoryType)
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
