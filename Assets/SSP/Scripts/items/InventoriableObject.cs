using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum InventoriableType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick }
public class InventoriableObject : NetworkBehaviour, IInteractable
{
    [SerializeField] private InventoriableType inventoriableType;
    [SerializeField] private bool canInteract = true;
    
    [SyncVar] public NetworkInstanceId ownerPlayerId;

    void Start()
    {
        if (ownerPlayerId != null)
            SetParent();
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
    private void SetParent()
    {
        var player = ClientScene.FindLocalObject(ownerPlayerId);
        if (player == null) return;
        transform.SetParent(player.GetComponent<PlayerInventoryManager>().rightHandTransform);
        transform.localPosition = Vector3.zero;
    }
}
