using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum InventoryType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick1, Gimmick2 }
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PlayerWeaponManager weaponManager;

    public Dictionary<InventoryType, InventoryWeapon> weapons { get; private set; }
    public InventoryType currentWeaponType { get; private set; }
    private int InventoryTypeCount;

    public void Init()
    {
        weapons = new Dictionary<InventoryType, InventoryWeapon>();
        InventoryTypeCount = Enum.GetNames(typeof(InventoryType)).Length;
    }

    public void AddWeapon(InventoryType type, InventoryWeapon weapon)
    {
        if (weapons.ContainsKey(type))
            weapons[type] = weapon;
        else
            weapons.Add(type, weapon);
    }

    public void EquipWeapon(InventoryType nextWeaponType)
    {
        //武器を持ち替えるので、持ち帰る前の武器は非表示に
        if (weapons.ContainsKey(currentWeaponType))
            weapons[currentWeaponType].gameObject.SetActive(false);

        currentWeaponType = nextWeaponType;
        weapons[nextWeaponType].gameObject.SetActive(true);
        weaponManager.attacker = weapons[nextWeaponType].attacker;
    }

    public void ReleaseWeapon(InventoryType releaseWeaponType)
    {
        if (weaponManager.attacker == weapons[releaseWeaponType].attacker)
            weaponManager.attacker = null;
        weapons[releaseWeaponType].gameObject.transform.parent = null;
        weapons[releaseWeaponType].gameObject.GetComponent<InventoriableObject>().SetCanInteract(true);
        weapons[releaseWeaponType].gameObject.SetActive(true);
    }

    public InventoryType GetNextWeaponType(InventoryType type)
    {
        var nextIndex = (int)type < InventoryTypeCount - 1 ? (int)type + 1 : 0;
        var nextType = (InventoryType)nextIndex;
        if (weapons.ContainsKey(nextType))
            return nextType;
        else
            return GetNextWeaponType(nextType);
    }

    public InventoryType GetPreviousWeaponType(InventoryType type)
    {
        int previousIndex = (int)type > 0 ? (int)type - 1 : InventoryTypeCount - 1;
        var previousType = (InventoryType)previousIndex;
        if (weapons.ContainsKey(previousType))
            return previousType;
        else
            return GetPreviousWeaponType(previousType);
    }

    public void ExchangeGimmicks()
    {
        var tmp = weapons[InventoryType.Gimmick1];
        weapons[InventoryType.Gimmick1] = weapons[InventoryType.Gimmick2];
        weapons[InventoryType.Gimmick2] = tmp;

        if (currentWeaponType == InventoryType.Gimmick1)
            currentWeaponType = InventoryType.Gimmick2;
        else if (currentWeaponType == InventoryType.Gimmick2)
            currentWeaponType = InventoryType.Gimmick1;
    }

}
