using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public enum InventoryType { LongRangeWeapon, ShortRangeWeapon, Gimmick }

    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;

    [SerializeField] private GameObject longRangeWeapon;
    [SerializeField] private GameObject shortRangeWeapon;
    [SerializeField] private List<GameObject> gimmicks = new List<GameObject>();
    [SerializeField] private int StockableGimmickCount = 2;

    public void SetLongRangeWeapon(GameObject go)
    {
        longRangeWeapon = go;
        SetObjectTransform(longRangeWeapon);
        SetWeaponToManager(go);
    }

    public void SetShortRangeWeapon(GameObject go)
    {
        shortRangeWeapon = go;
        SetObjectTransform(shortRangeWeapon);
        SetWeaponToManager(go);
    }

    public void AddGimmick(GameObject go)
    {
        if (gimmicks.Count >= StockableGimmickCount)
        {
            var removeGimmick = gimmicks[0];
            ReleaseObject(removeGimmick);
            gimmicks.RemoveAt(0);
        }
        gimmicks.Add(go);
        SetObjectTransform(go);
        SetWeaponToManager(go);
    }

    private void SetObjectTransform(GameObject go)
    {
        go.transform.parent = rightHand.transform;
        go.transform.localPosition = Vector3.zero;
    }

    private void SetWeaponToManager(GameObject go)
    {
        var attacker = go.GetComponent<IAttackable>();
        if (attacker != null)
        {
            if (!weaponManager.ExistAttacker())
                weaponManager.SetAttacker(attacker);
            else
                go.SetActive(false);
        }
    }

    private void ReleaseObject(GameObject go)
    {
        go.SetActive(true);
        go.transform.parent = null;
        go.GetComponent<InventoriableObject>().SetCanInteract(true);
    }
}
