using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerInventoryManager : NetworkBehaviour
{
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] public PlayerInventory inventory;
    [SerializeField] private GameObject handGunPrefab;
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    void Start()
    {
        //ハンドガンは初期状態から所持。
        if (isLocalPlayer)
        {
            //ホストはNetworkConnectionの問題、クライアントはコルーチンを挟むとなぜかエラーが出る。
            //なのでホストとクライアントで処理をわける
            if (isServer)
                StartCoroutine(SetUpHandGun());
            else if (isClient)
                CmdSetupDefaultWeapon();
        }

        pim.WeaponChange
            .Subscribe(v =>
            {
                if (v < 0)
                {
                    var nextWeaponType = inventory.GetNextWeaponType();
                    CmdChangeWeapon((int)nextWeaponType);
                }
                else if (v > 0)
                {
                    var previousWeaponType = inventory.GetPreviousWeaponType();
                    CmdChangeWeapon((int)previousWeaponType);
                }
            });
    }

    public void SetWeaponToInventory(GameObject go, WeaponType weaponType)
    {
        var type = ConvertInventoriableTypeToInventoryType(weaponType);

        //(拾ったギミック) -> Gimmick2 -> Gimmick1 -> (捨てる) という感じで、ギミックがいっぱいの時はギミックを押し出す。
        if (type == InventoryType.Gimmick2 && inventory.HasWeapon(InventoryType.Gimmick2))
            inventory.SwapGimmicks();

        if (inventory.HasWeapon(type))
            inventory.ReleaseWeapon(type);

        inventory.SetWeapon(type, go);

        //装備中の武器と同種の武器がきた場合は装備しなおす。
        //ただし、typeがGimmick1の時はSetされる前にGimmick2だったものであり、まだ所持しているので装備しなおさない。
        if (type == inventory.currentWeaponType && type != InventoryType.Gimmick1)
            inventory.EquipWeapon(type);
    }

    #region InventoryType変換
    private Dictionary<WeaponType, InventoryType> inventoryTypeMap
        = new Dictionary<WeaponType, InventoryType>()
        {
            {WeaponType.HandGun, InventoryType.HandGun },
            {WeaponType.LongRangeWeapon, InventoryType.LongRangeWeapon },
            {WeaponType.ShortRangeWeapon,InventoryType.ShortRangeWeapon },
            {WeaponType.Gimmick, InventoryType.Gimmick1 }
        };

    public InventoryType ConvertInventoriableTypeToInventoryType(WeaponType weaponType)
    {
        InventoryType type = inventoryTypeMap[weaponType];
        //Gimmick1に格納するのは最初にGimmickを拾ったときだけ。2回目以降はGimmick2のギミックをGimmick1に移動してGimmick2に新しいギミックを格納する
        if (type == InventoryType.Gimmick1)
            inventoryTypeMap[WeaponType.Gimmick] = InventoryType.Gimmick2;
        return type;
    }
    #endregion

    #region ハンドガン初期セットアップ処理
    IEnumerator SetUpHandGun()
    {
        //ホストのみNetworkConnectionが確立される前にStartが呼び出されてしまうため、NetworkConnectionが確立するまで待つ。
        yield return this.UpdateAsObservable().FirstOrDefault(_ => connectionToClient.isReady).ToYieldInstruction();
        CmdSetupDefaultWeapon();
    }

    [Command]
    private void CmdSetupDefaultWeapon()
    {
        var handGunObj = Instantiate(handGunPrefab);
        NetworkServer.SpawnWithClientAuthority(handGunObj, connectionToClient);
        RpcAssignPlayerToDefaultWeapon(handGunObj.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    private void RpcAssignPlayerToDefaultWeapon(NetworkInstanceId instanceId)
    {
        var weapon = ClientScene.FindLocalObject(instanceId);
        var invObj = weapon.GetComponent<InventoriableObject>();
        invObj.ownerPlayerId = GetComponent<NetworkIdentity>().netId;
    }
    #endregion

    #region 武器持ち替え処理
    [Command]
    void CmdChangeWeapon(int inventoryTypeIndex)
    {
        RpcChangeWeapon(inventoryTypeIndex);
    }

    [ClientRpc]
    void RpcChangeWeapon(int inventoryTypeIndex)
    {
        inventory.EquipWeapon((InventoryType)inventoryTypeIndex);
    }
    #endregion
}
