using System.Collections;
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
        public InventoryType type;
        public InventoryWeapon(GameObject go, InventoryType _type)
        {
            gameObject = go;
            attacker = go.GetComponent<IAttackable>();
            type = _type;
        }
    }

    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject handGunPrefab;
    
    private InventoryWeapon handGun;
    private InventoryWeapon longRangeWeapon;
    private InventoryWeapon shortRangeWeapon;
    private InventoryWeapon gimmick1;
    private InventoryWeapon gimmick2;

    [SerializeField] private InventoryWeapon currentWeapon;

    private void Start()
    {
        handGun = new InventoryWeapon(handGunPrefab, InventoryType.HandGun);
        SetCurrentWeapon(handGun);

        pim.WeaponChange
            .Subscribe(v =>
            {
                if (v > 0)
                    ChangeNextWeapon(currentWeapon.type);
                //else if (v < 0)

            });
    }

    public void SetWeapon(GameObject go, InventoryType type)
    {
        var weapon = new InventoryWeapon(go, type);
        SetWeaponTransform(weapon.gameObject);
        weapon.gameObject.SetActive(false);
        switch (type)
        {
            case InventoryType.LongRangeWeapon:
                if (longRangeWeapon.gameObject != null)
                    ReleaseWeapon(longRangeWeapon.gameObject);
                longRangeWeapon = weapon;
                break;
            case InventoryType.ShortRangeWeapon:
                if (shortRangeWeapon.gameObject != null)
                    ReleaseWeapon(shortRangeWeapon.gameObject);
                shortRangeWeapon = weapon;
                break;
            case InventoryType.Gimmick1:
                if (gimmick1.gameObject != null)
                    ReleaseWeapon(gimmick1.gameObject);
                gimmick1 = weapon;
                break;
            case InventoryType.Gimmick2:
                if (gimmick2.gameObject != null)
                    ReleaseWeapon(gimmick2.gameObject);
                gimmick2 = weapon;
                break;
        }
    }

    private void SetWeaponTransform(GameObject go)
    {
        go.transform.parent = rightHand.transform;
        go.transform.localPosition = Vector3.zero;
    }

    private void SetCurrentWeapon(InventoryWeapon weapon)
    {
        if (currentWeapon.gameObject != null)
            currentWeapon.gameObject.SetActive(false);

        currentWeapon = weapon;
        weapon.gameObject.SetActive(true);
        weaponManager.SetAttacker(weapon.attacker);
    }

    private void ReleaseWeapon(GameObject go)
    {
        go.SetActive(true);
        go.transform.parent = null;
        go.GetComponent<InventoriableObject>().SetCanInteract(true);
    }

    private void ChangeNextWeapon(InventoryType type)
    {
        switch (type)
        {
            case InventoryType.LongRangeWeapon:
                if (shortRangeWeapon.gameObject != null)
                    SetCurrentWeapon(shortRangeWeapon);
                else
                    ChangeNextWeapon(InventoryType.ShortRangeWeapon);
                break;
            case InventoryType.ShortRangeWeapon:
                if (gimmick1.gameObject != null)
                    SetCurrentWeapon(gimmick1);
                else
                    ChangeNextWeapon(InventoryType.Gimmick1);
                break;
            case InventoryType.Gimmick1:
                if (gimmick2.gameObject != null)
                    SetCurrentWeapon(gimmick2);
                else
                    ChangeNextWeapon(InventoryType.Gimmick2);
                break;
            case InventoryType.Gimmick2:
                if (handGun.gameObject != null)
                    SetCurrentWeapon(handGun);
                else
                    ChangeNextWeapon(InventoryType.HandGun);
                break;
            case InventoryType.HandGun:
                if (longRangeWeapon.gameObject != null)
                    SetCurrentWeapon(longRangeWeapon);
                else
                    ChangeNextWeapon(InventoryType.LongRangeWeapon);
                break;
        }
    }

    private void ChangePreviousWeapon()
    {
        switch (currentWeaponType)
        {
            case InventoryType.LongRangeWeapon:
                SetCurrentWeapon(handGun);
                break;
            case InventoryType.ShortRangeWeapon:
                SetCurrentWeapon(longRangeWeapon);
                break;
            case InventoryType.Gimmick:
                SetCurrentWeapon(shortRangeWeapon);
                break;
            case InventoryType.HandGun:
                SetCurrentWeapon(gimmicks[0]);
                break;
        }
    }
}
