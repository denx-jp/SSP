using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerInventoryManager : NetworkBehaviour
{
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private GameObject handGunPrefab;
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    void Start()
    {
        inventory.Init();

        //ハンドガンは初期状態から所持。
        if (isLocalPlayer)
        {
            //ホストはNetworkConnectionの問題、クライアントはコルーチンを挟むとなぜかエラーが出る。
            //なのでホストとクライアントで処理をわける
            if (isServer)
                StartCoroutine(SetUpHandGun());
            else if (isClient)
                CmdSetupHandGun();
        }

        pim.WeaponChange
            .Subscribe(v =>
            {
                if (v < 0)
                    CmdNextWeapon();
                else if (v > 0)
                    CmdPreviousWeapon();
            });
    }

    public void SetWeaponToInventory(GameObject go, InventoriableType inventoriableType)
    {
        var type = ConvertInventoriableTypeToInventoryType(inventoriableType);
        if (inventory.weapons.ContainsKey(type))
            inventory.ReleaseWeapon(type);

        var weapon = new InventoryWeapon(go);
        weapon.attacker.Init(playerModel);
        weapon.gameObject.SetActive(false);
        
        inventory.AddWeapon(type, weapon);
        //Gimmick1の時は入れ替える前Gimmick2だったもので、まだ所持しているので装備しなおさない
        if (type == inventory.currentWeaponType && type != InventoryType.Gimmick1)
            inventory.EquipWeapon(type);
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

    #region ハンドガン初期セットアップ処理
    IEnumerator SetUpHandGun()
    {
        //ホストのみNetworkConnectionが確立される前にStartが呼び出されてしまうため、NetworkConnectionが確立するまで待つ。
        yield return this.UpdateAsObservable().FirstOrDefault(_ => connectionToClient.isReady).ToYieldInstruction();
        CmdSetupHandGun();
    }

    [Command]
    private void CmdSetupHandGun()
    {
        var handGunObj = Instantiate(handGunPrefab);
        NetworkServer.SpawnWithClientAuthority(handGunObj, connectionToClient);
        RpcSetupHandGun(handGunObj.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    private void RpcSetupHandGun(NetworkInstanceId instanceId)
    {
        var weapon = ClientScene.FindLocalObject(instanceId);
        var invObj = weapon.GetComponent<InventoriableObject>();
        invObj.ownerPlayerId = GetComponent<NetworkIdentity>().netId;
        SetDefaultWeapon(weapon, invObj.inventoriableType);
    }

    public void SetDefaultWeapon(GameObject weapon, InventoriableType type)
    {
        SetWeaponToInventory(weapon, type);
        var inventoryType = ConvertInventoriableTypeToInventoryType(type);
        inventory.EquipWeapon(inventoryType);
    }
    #endregion

    #region 武器持ち替え処理
    [Command]
    void CmdNextWeapon()
    {
        RpcNextWeapon();
    }

    [ClientRpc]
    void RpcNextWeapon()
    {
        var nextWeaponType = inventory.GetNextWeaponType();
        inventory.EquipWeapon(nextWeaponType);
    }

    [Command]
    void CmdPreviousWeapon()
    {
        RpcPreviousWeapon();
    }

    [ClientRpc]
    void RpcPreviousWeapon()
    {
        var previousWeaponType = inventory.GetPreviousWeaponType();
        inventory.EquipWeapon(previousWeaponType);
    }
    #endregion
}
