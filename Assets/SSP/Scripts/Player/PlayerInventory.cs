using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerInventory : MonoBehaviour
{
    public enum InventoryType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick1, Gimmick2 }
    private struct InventoryWeapon
    {
        public GameObject gameObject;
        public IAttackable attacker;
        public InventoryWeapon(GameObject go)
        {
            gameObject = go;
            attacker = go.GetComponent<IAttackable>();
        }
    }

    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject handGunPrefab;

    private Dictionary<InventoryType, InventoryWeapon> weapons;
    private InventoryType currentWeaponType;
    private int InventoryTypeCount;

    private void Start()
    {
        weapons = new Dictionary<InventoryType, InventoryWeapon>();
        InventoryTypeCount = Enum.GetNames(typeof(InventoryType)).Length;

        var handGunObj = Instantiate(handGunPrefab);
        SetWeapon(handGunObj, InventoryType.HandGun);
        SetCurrentWeapon(InventoryType.HandGun);

        pim.WeaponChange
            .Subscribe(v =>
            {
                if (v > 0)
                    ChangeNextWeapon(currentWeaponType);
                else if (v < 0)
                    ChangePreviousWeapon(currentWeaponType);
            });
    }

    public void SetWeapon(GameObject go, InventoryType type)
    {
        Debug.Log($"set : {type.ToString()}");
        var weapon = new InventoryWeapon(go);
        if (weapons.ContainsKey(type))
        {
            ReleaseWeapon(weapons[type].gameObject);
            weapons[type] = weapon;
        }
        else
            weapons.Add(type, weapon);

        go.transform.parent = rightHand.transform;      //後々WeaponModelみたいなのを作って手に対する位置などを保存して、そこから設定するように
        go.transform.localPosition = Vector3.zero;         //同上
        weapon.gameObject.SetActive(false);
    }

    private void SetCurrentWeapon(InventoryType type)
    {
        Debug.Log($"set current : {type.ToString()}");
        if (weapons[currentWeaponType].gameObject != null)
            weapons[currentWeaponType].gameObject.SetActive(false);

        currentWeaponType = type;
        weapons[type].gameObject.SetActive(true);
        weaponManager.SetAttacker(weapons[type].attacker);
    }

    private void ReleaseWeapon(GameObject go)
    {
        go.SetActive(true);
        go.transform.parent = null;
        go.GetComponent<InventoriableObject>().SetCanInteract(true);
    }

    private void ChangeNextWeapon(InventoryType type)
    {
        var nextIndex = (int)type < InventoryTypeCount ? (int)type + 1 : 0;
        var nextType = (InventoryType)nextIndex;
        if (weapons.ContainsKey(nextType))
            SetCurrentWeapon(nextType);
        else
            ChangeNextWeapon(nextType);
    }

    private void ChangePreviousWeapon(InventoryType type)
    {
        int previousIndex = (int)type > 0 ? (int)type - 1 : InventoryTypeCount - 1;
        var previousType = (InventoryType)previousIndex;
        if (weapons.ContainsKey(previousType))
            SetCurrentWeapon(previousType);
        else
            ChangePreviousWeapon(previousType);
    }
}
