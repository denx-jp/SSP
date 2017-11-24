using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class CrossViewModel : MonoBehaviour
{
    [SerializeField] Image image;
    private PlayerModel playerModel;
    private PlayerInventory inventory;

    public void Init(PlayerManager playerManager)
    {
        playerModel = playerManager.playerModel;
        inventory = playerManager.playerInventory;

        this.ObserveEveryValueChanged(_=> playerModel.MoveMode)
            .Subscribe(mode =>
            {
                if (mode == MoveMode.battle && (inventory.currentWeaponType == InventoryType.HandGun || inventory.currentWeaponType == InventoryType.LongRangeWeapon))
                    image.enabled = true;
                else
                    image.enabled = false;
            });
    }
}
