using UnityEngine;

public struct InventoryWeapon
{
    public GameObject gameObject;
    public IWeapon weapon;
    public InventoryWeapon(GameObject go)
    {
        gameObject = go;
        weapon = go.GetComponent<IWeapon>();
    }
}
