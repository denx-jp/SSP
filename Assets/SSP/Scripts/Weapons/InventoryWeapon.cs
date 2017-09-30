using UnityEngine;

public struct InventoryWeapon
{
    public GameObject gameObject;
    public IAttackable attacker;
    public InventoryWeapon(GameObject go)
    {
        gameObject = go;
        attacker = go.GetComponent<IAttackable>();
    }
}
