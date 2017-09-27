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
        public InventoryWeapon(GameObject go, PlayerModel playerModel)
        {
            gameObject = go;
            attacker = go.GetComponent<IAttackable>();
            attacker.Init(playerModel);
        }
    }

    [SerializeField] private PlayerModel playerModel;
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
        var weapon = new InventoryWeapon(go, playerModel);
        //初めてそのtypeの武器を拾った時はDictionaryにAddする。２回目以降は古い武器を捨てて、新しい武器を入れる。
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

    private void SetCurrentWeapon(InventoryType nextWeaponType)
    {
        //武器を持ち替えるので、持ち帰る前の武器は非表示に
        if (weapons.ContainsKey(currentWeaponType))
            weapons[currentWeaponType].gameObject.SetActive(false);

        currentWeaponType = nextWeaponType;
        weapons[nextWeaponType].gameObject.SetActive(true);
        weaponManager.SetAttacker(weapons[nextWeaponType].attacker);
    }

    private void ReleaseWeapon(GameObject go)
    {
        go.SetActive(true);
        go.transform.parent = null;
        go.GetComponent<InventoriableObject>().SetCanInteract(true);
    }

    private void ChangeNextWeapon(InventoryType type)
    {
        var nextIndex = (int)type < InventoryTypeCount - 1 ? (int)type + 1 : 0;
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
