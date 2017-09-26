using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public enum InventoryType { LongRangeWeapon, ShortRangeWeapon, Gimmick }

    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;

    [SerializeField] private GameObject longRangeWeapon;
    [SerializeField] private GameObject shortRangeWeapon;
    [SerializeField] private List<GameObject> gimmicks = new List<GameObject>();
    [SerializeField] private int StockableGimmickCount = 2;

    public void SetLongRangeWeapon(GameObject go)
    {
        Debug.Log("long");
        longRangeWeapon = go;
        longRangeWeapon.transform.parent = rightHand.transform;
        longRangeWeapon.transform.localPosition = Vector3.zero;
    }

    public void SetShortRangeWeapon(GameObject go)
    {
        Debug.Log("short");
        shortRangeWeapon = go;
        shortRangeWeapon.transform.parent = rightHand.transform;
        shortRangeWeapon.transform.localPosition = Vector3.zero;
    }

    public void AddGimmick(GameObject go)
    {
        if (gimmicks.Count < StockableGimmickCount)
            gimmicks.Add(go);
    }

}
