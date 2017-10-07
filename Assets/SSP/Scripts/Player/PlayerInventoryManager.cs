using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerInventoryManager : NetworkBehaviour
{

    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] public Transform rightHandTransform;
    [SerializeField] public Transform leftHandTransform;
    [SerializeField] private GameObject handGunPrefab;

    [Command]
    void CmdHandGun()
    {
        var handGunObj = Instantiate(handGunPrefab);
        NetworkServer.SpawnWithClientAuthority(handGunObj, connectionToClient);
        RpcHandGun(handGunObj.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    void RpcHandGun(NetworkInstanceId instanceId)
    {
        var weapon = ClientScene.FindLocalObject(instanceId);
        weapon.GetComponent<InventoriableObject>().ownerPlayerId = GetComponent<NetworkIdentity>().netId;
        SetWeaponToInventory(weapon, InventoriableType.HandGun);
        inventory.EquipWeapon(InventoryType.HandGun);
    }

    void Start()
    {
        inventory.Init();
        //ハンドガンは初期状態から所持する仕様
        if (isLocalPlayer)
            CmdHandGun();

        pim.WeaponChange
            .Subscribe(v =>
            {
                if (v < 0)
                {
                    var nextWeaponType = inventory.GetNextWeaponType();
                    inventory.EquipWeapon(nextWeaponType);
                }
                else if (v > 0)
                {
                    var previousWeaponType = inventory.GetPreviousWeaponType();
                    inventory.EquipWeapon(previousWeaponType);
                }
            });
    }

    public void SetWeaponToInventory(GameObject go, InventoriableType inventoriableType)
    {
        InventoryType type = ConvertInventoriableTypeToInventoryType(inventoriableType);
        if (inventory.weapons.ContainsKey(type))
            inventory.ReleaseWeapon(type);

        var weapon = new InventoryWeapon(go);
        weapon.attacker.Init(playerModel);

        weapon.gameObject.transform.parent = rightHandTransform;
        weapon.gameObject.transform.localPosition = Vector3.zero;
        weapon.gameObject.SetActive(false);     //インベントリの中の武器は非表示に

        inventory.AddWeapon(type, weapon);
        if (type == inventory.currentWeaponType && type != InventoryType.Gimmick1)  //Gimmick1の時は入れ替える前Gimmick2だったもので、まだ所持しているので装備しなおさない
        {
            inventory.EquipWeapon(type);
        }
    }

    #region enum変換
    private Dictionary<InventoriableType, InventoryType> inventoryTypeMap
        = new Dictionary<InventoriableType, InventoryType>()
        {
            {InventoriableType.HandGun, InventoryType.HandGun },
            {InventoriableType.LongRangeWeapon, InventoryType.LongRangeWeapon },
            {InventoriableType.ShortRangeWeapon,InventoryType.ShortRangeWeapon },
            {InventoriableType.Gimmick, InventoryType.Gimmick1 }
        };
    private InventoryType ConvertInventoriableTypeToInventoryType(InventoriableType inventoriableType)
    {
        InventoryType type = inventoryTypeMap[inventoriableType];

        //初回のみGimmick1に収納。以降はGimmick1を押し出すようにするため、G1とG2を入れ替えてからG2を返す。
        if (type == InventoryType.Gimmick1)
            inventoryTypeMap[InventoriableType.Gimmick] = InventoryType.Gimmick2;
        if (type == InventoryType.Gimmick2 && inventory.weapons.ContainsKey(InventoryType.Gimmick2))
            inventory.SwapGimmicks();

        return type;
    }
    #endregion

    private void SetParent(GameObject weaponObj)
    {
        weaponObj.transform.parent = rightHandTransform;
        weaponObj.transform.localPosition = Vector3.zero;
    }

}
