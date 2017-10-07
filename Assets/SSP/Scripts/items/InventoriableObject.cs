﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoriableType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick }
public class InventoriableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoriableType inventoriableType;
    [SerializeField] private bool canInteract = true;
    [SerializeField] public Vector3 weaponPos;
    [SerializeField] public Quaternion weaponRotate;

    [SerializeField] public bool isLeftHand;

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
}
