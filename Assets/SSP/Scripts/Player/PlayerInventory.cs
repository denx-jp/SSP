using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private struct Weapon
    {
        public GameObject gameObject;
        public IAttackable attacker;
        public Weapon(GameObject go, IAttackable atk)
        {
            gameObject = go;
            attacker = atk;
        }
    }
    public enum InventoryType { LongRangeWeapon, ShortRangeWeapon, Gimmick }

    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;

    [SerializeField] private Weapon longRangeWeapon;
    [SerializeField] private Weapon shortRangeWeapon;
    [SerializeField] private List<Weapon> gimmicks = new List<Weapon>();
    [SerializeField] private int StockableGimmickCount = 2;

    private InventoryType currentWeaponType;

    public void SetWeapon(GameObject go, InventoryType type)
    {
        var inventoryWeapon = new Weapon(go, go.GetComponent<IAttackable>());
        currentWeaponType = type;
        SetObjectTransform(inventoryWeapon.gameObject);
        SetWeaponToManager(inventoryWeapon);
        switch (type)
        {
            case InventoryType.LongRangeWeapon:
                longRangeWeapon = inventoryWeapon;
                break;
            case InventoryType.ShortRangeWeapon:
                shortRangeWeapon = inventoryWeapon;
                break;
            case InventoryType.Gimmick:
                if (gimmicks.Count >= StockableGimmickCount)
                {
                    var removeGimmick = gimmicks[0];
                    ReleaseObject(removeGimmick.gameObject);
                    gimmicks.RemoveAt(0);
                }
                gimmicks.Add(inventoryWeapon);
                break;
        }
    }

    {
        {
        }
    }

    private void SetObjectTransform(GameObject go)
    {
        go.transform.parent = rightHand.transform;
        go.transform.localPosition = Vector3.zero;
    }

    private void SetWeaponToManager(Weapon weapon)
    {
        if (weapon.attacker != null)
        {
            if (!weaponManager.ExistAttacker())
                weaponManager.SetAttacker(weapon.attacker);
            else
                weapon.gameObject.SetActive(false);
        }
    }

    private void ReleaseObject(GameObject go)
    {
        go.SetActive(true);
        go.transform.parent = null;
        go.GetComponent<InventoriableObject>().SetCanInteract(true);
    }
}
