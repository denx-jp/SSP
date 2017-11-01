using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum InventoryType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick1, Gimmick2 }
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerWeaponManager weaponManager;

    public Dictionary<InventoryType, InventoryWeapon> weapons { get; private set; } = new Dictionary<InventoryType, InventoryWeapon>();
    private int inventoryTypeCount = Enum.GetNames(typeof(InventoryType)).Length;
    public InventoryType currentWeaponType { get; private set; }

    private Subject<WeaponModel> WeaponStream = new Subject<WeaponModel>();

    public void SetWeapon(InventoryType type, GameObject weaponObj)
    {
        var inventoryWeapon = new InventoryWeapon(weaponObj);
        inventoryWeapon.weapon.Init(playerModel);
        inventoryWeapon.gameObject.SetActive(false);
        weapons[type] = inventoryWeapon;
    }

    public void ReleaseWeapon(InventoryType releaseWeaponType)
    {
        if (currentWeaponType == releaseWeaponType)
            weaponManager.weapon = null;
        weapons[releaseWeaponType].gameObject.transform.parent = null;
        weapons[releaseWeaponType].gameObject.GetComponent<InventoriableObject>().SetCanInteract(true);
        weapons[releaseWeaponType].gameObject.SetActive(true);
    }

    public void EquipWeapon(InventoryType nextWeaponType)
    {
        //武器を持ち替えるので、持ち帰る前の武器は非表示に
        if (HasWeapon(currentWeaponType))
            weapons[currentWeaponType].gameObject.SetActive(false);

        currentWeaponType = nextWeaponType;
        weapons[nextWeaponType].gameObject.SetActive(true);
        weaponManager.weapon = weapons[nextWeaponType].weapon;
    }

    public InventoryType GetNextWeaponType()
    {
        var currentIndex = (int)currentWeaponType;
        for (int i = 1; i <= inventoryTypeCount; i++)
        {
            var nextIndex = currentIndex + i < inventoryTypeCount ? currentIndex + i : currentIndex + i - inventoryTypeCount;
            var nextType = (InventoryType)nextIndex;
            if (HasWeapon(nextType))
                return nextType;
        }
        return currentWeaponType;
    }

    public InventoryType GetPreviousWeaponType()
    {
        var currentIndex = (int)currentWeaponType;
        for (int i = 1; i <= inventoryTypeCount; i++)
        {
            var previousIndex = currentIndex - i >= 0 ? currentIndex - i : currentIndex - i + inventoryTypeCount;
            var previousType = (InventoryType)previousIndex;
            if (HasWeapon(previousType))
                return previousType;
        }
        return currentWeaponType;
    }

    public void SwapGimmicks()
    {
        var tmp = weapons[InventoryType.Gimmick1];
        weapons[InventoryType.Gimmick1] = weapons[InventoryType.Gimmick2];
        weapons[InventoryType.Gimmick2] = tmp;

        if (currentWeaponType == InventoryType.Gimmick1)
            currentWeaponType = InventoryType.Gimmick2;
        else if (currentWeaponType == InventoryType.Gimmick2)
            currentWeaponType = InventoryType.Gimmick1;
    }

    public bool HasWeapon(InventoryType type)
    {
        return weapons.ContainsKey(type);
    }
}
