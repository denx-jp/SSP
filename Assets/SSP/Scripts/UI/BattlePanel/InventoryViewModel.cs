using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class InventoryViewModel : MonoBehaviour
{
    [SerializeField] private List<WeaponViewModel> weapoVMs = new List<WeaponViewModel>();

    private PlayerInventory inventory;
    private Dictionary<InventoryType, WeaponViewModel> ViewModelMap = new Dictionary<InventoryType, WeaponViewModel>();

    void Start()
    {
        foreach (var weaponVM in weapoVMs)
        {
            ViewModelMap[weaponVM.type] = weaponVM;
        }
    }

    public void Init(PlayerInventory _inventory)
    {
        inventory = _inventory;

        // Initされる前から所持している武器のViewを更新
        inventory.weapons.ToObservable().Subscribe(v => UpdateView(v.Key, v.Value.model.image));

        inventory.weapons.ObserveReplace().Subscribe(v => UpdateView(v.Key, v.NewValue.model.image));
        inventory.weapons.ObserveAdd().Subscribe(v => UpdateView(v.Key, v.Value.model.image));
        inventory.weapons.ObserveRemove().Subscribe(v => UpdateView(v.Key, null));

        this.ObserveEveryValueChanged(_ => inventory.currentWeaponType)
            .Subscribe(type =>
            {
                foreach (var weaponVMPair in ViewModelMap)
                {
                    bool active = weaponVMPair.Key == type;
                    weaponVMPair.Value.SwapBackgroundColor(active);
                }
            });
    }

    private void UpdateView(InventoryType type, Sprite image)
    {
        ViewModelMap[type].SetImage(image);
    }
}
