using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerInventoryManager : MonoBehaviour
{
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject handGunPrefab;
    private InventoriableObject invObject;

    void Start()
    {
        inventory.Init();

        //ハンドガンは初期状態から所持する仕様
        var handGunObj = Instantiate(handGunPrefab);
        SetWeaponToInventory(handGunObj, InventoriableType.HandGun);
        inventory.EquipWeapon(InventoryType.HandGun);

        pim.WeaponChange
            .Subscribe(v =>
            {
                if (v < 0)
                {
                    var nextWeaponType = inventory.GetNextWeaponType();
                    inventory.EquipWeapon(nextWeaponType);
                }
                else if (v > 0)
                {
                    var previousWeaponType = inventory.GetPreviousWeaponType();
                    inventory.EquipWeapon(previousWeaponType);
                }
            });
    }

    public void SetWeaponToInventory(GameObject go, InventoriableType inventoriableType)
    {
        InventoryType type = ConvertInventoriableTypeToInventoryType(inventoriableType);
        if (inventory.weapons.ContainsKey(type))
            inventory.ReleaseWeapon(type);

        var weapon = new InventoryWeapon(go);
        invObject = weapon.gameObject.GetComponent<InventoriableObject>();
        weapon.attacker.Init(playerModel);
        invObject.HoldHand(leftHand, rightHand);
        invObject.SetWeaponPosition();
        weapon.gameObject.SetActive(false);
        inventory.AddWeapon(type, weapon);
        if (type == inventory.currentWeaponType && type != InventoryType.Gimmick1)
        {
            inventory.EquipWeapon(type);
        }
    }

    private InventoryType ConvertInventoriableTypeToInventoryType(InventoriableType inventoriableType)
    {
        InventoryType type;
        switch (inventoriableType)
        {
            case InventoriableType.HandGun:
                type = InventoryType.HandGun;
                break;
            case InventoriableType.LongRangeWeapon:
                type = InventoryType.LongRangeWeapon;
                break;
            case InventoriableType.ShortRangeWeapon:
                type = InventoryType.ShortRangeWeapon;
                break;
            default:
                if (!inventory.weapons.ContainsKey(InventoryType.Gimmick1) && !inventory.weapons.ContainsKey(InventoryType.Gimmick2))
                    type = InventoryType.Gimmick1;
                else if (inventory.weapons.ContainsKey(InventoryType.Gimmick1) && !inventory.weapons.ContainsKey(InventoryType.Gimmick2))
                    type = InventoryType.Gimmick2;
                else
                {
                    //Gimmick1もGimmick2も所持している場合はGimmick1を押し出すようにするため、
                    //Gimmick1とGimmick2を入れ替えてからGimmick2返す
                    inventory.SwapGimmicks();
                    type = InventoryType.Gimmick2;
                }
                break;
        }
        return type;
    }

}
