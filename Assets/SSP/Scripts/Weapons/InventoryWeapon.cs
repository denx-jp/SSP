using UnityEngine;

public struct InventoryWeapon
{
    public GameObject gameObject;
    public IWeapon weapon;
    public WeaponModel model;
    public InventoryWeapon(GameObject go)
    {
        gameObject = go;
        weapon = go.GetComponent<IWeapon>();
        model = go.GetComponent<WeaponModel>();
    }
}
