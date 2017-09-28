using UnityEngine;

public struct InventoryWeapon
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
