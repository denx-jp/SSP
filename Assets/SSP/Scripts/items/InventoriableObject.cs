using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class InventoriableObject : NetworkBehaviour, IInteractable
{
    [SerializeField] public Vector3 weaponPos;
    [SerializeField] public Vector3 weaponRot;
    [HideInInspector] public WeaponModel model;
    [SerializeField] public bool canInteract = true;

    private enum Hands { leftHand, rightHand };
    [SerializeField] private Hands hand;

    [SyncVar] public NetworkInstanceId ownerPlayerId;
    private NetworkIdentity networkIdentity;

    private List<Collider> colliders;
    private Rigidbody rigid;

    void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        model = GetComponent<WeaponModel>();
        colliders = GetComponentsInChildren<Collider>().Where(v => !v.isTrigger).ToList();
        rigid = GetComponent<Rigidbody>();

        StartCoroutine(DefaultWeaponSetup());
    }

    [Server]
    public void Interact(PlayerManager pm)
    {
        RpcSetToInventory(pm.gameObject);
        networkIdentity.AssignClientAuthority(pm.connectionToClient);
    }

    [ClientRpc]
    void RpcSetToInventory(GameObject player)
    {
        SetPhysicsSettings(false);

        ownerPlayerId = player.GetComponent<NetworkIdentity>().netId;
        var pm = player.GetComponent<PlayerManager>();
        SetTransformOwnerHand(pm.playerInventoryManager.leftHandTransform, pm.playerInventoryManager.rightHandTransform);
        pm.playerInventoryManager.SetWeaponToInventory(this.gameObject, model.type);
        pm.playerAnimationController.Pickup();
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public void Release()
    {
        SetPhysicsSettings(true);
    }

    //所持中の武器を持ち主のインベントリに格納・装備する
    [ClientCallback]
    IEnumerator DefaultWeaponSetup()
    {
        yield return new WaitForSeconds(1);     // ownerPlayerId(SyncVar)の同期に少し時間がかかるらしいので待つ。

        var owner = ClientScene.FindLocalObject(ownerPlayerId);
        if (owner == null) yield break;
        var pim = owner.GetComponent<PlayerInventoryManager>();
        SetPhysicsSettings(false);

        pim.SetWeaponToInventory(this.gameObject, model.type);
        var inventoryType = pim.ConvertInventoriableTypeToInventoryType(model.type);
        if (pim.inventory.currentWeaponType == inventoryType)
            pim.inventory.EquipWeapon(inventoryType);
        SetTransformOwnerHand(pim.leftHandTransform, pim.rightHandTransform);
    }

    private void SetTransformOwnerHand(Transform leftHand, Transform rightHand)
    {
        switch (hand)
        {
            case Hands.leftHand:
                transform.parent = leftHand;
                break;
            case Hands.rightHand:
                transform.parent = rightHand;
                break;
        }

        transform.localPosition = weaponPos;
        transform.localRotation = Quaternion.Euler(weaponRot.x, weaponRot.y, weaponRot.z);
    }

    private void SetPhysicsSettings(bool enable)
    {
        canInteract = enable;
        colliders.ForEach(v => v.enabled = enable);
        rigid.useGravity = enable;
        rigid.isKinematic = !enable;
    }
}
