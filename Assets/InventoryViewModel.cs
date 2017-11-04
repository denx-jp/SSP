using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InventoryViewModel : MonoBehaviour
{
    [HideInInspector] public PlayerInventory inventory;
    [SerializeField] private List<WeaponViewModel> weaponVM = new List<WeaponViewModel>();
    [SerializeField] private Sprite defaultImage;

    private Dictionary<InventoryType, WeaponViewModel> viewMap = new Dictionary<InventoryType, WeaponViewModel>();

    void Start()
    {
        foreach (var view in weaponVM)
        {
            viewMap[view.type] = view;
            view.SetImage(defaultImage);
        }
    }

    public void Init()
    {
        // Initされる前から所持している武器のViewを更新
        inventory.weapons.ToObservable().Subscribe(v => UpdateView(v.Key, v.Value.model));

        inventory.weapons.ObserveReplace().Subscribe(v => UpdateView(v.Key, v.NewValue.model));
        inventory.weapons.ObserveAdd().Subscribe(v => UpdateView(v.Key, v.Value.model));
        //inventory.weapons.ObserveRemove().Subscribe(v => UpdateView(v.Key, defaultModel));
    }

    private void UpdateView(InventoryType type, WeaponModel model)
    {
        var view = viewMap[type];
        view.SetImage(model.image);
    }
}
