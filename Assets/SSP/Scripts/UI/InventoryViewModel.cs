using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class InventoryViewModel : MonoBehaviour
{
    private struct WeaponPanel
    {
        public WeaponViewModel weaponVM;
        public Outline outline;
        public WeaponPanel(WeaponViewModel vm, Outline ol)
        {
            weaponVM = vm;
            outline = ol;
        }
    }

    [HideInInspector] public PlayerInventory inventory;
    [SerializeField] private List<GameObject> weaponPanels = new List<GameObject>();
    [SerializeField] private Sprite defaultImage;

    private Dictionary<InventoryType, WeaponPanel> viewMap = new Dictionary<InventoryType, WeaponPanel>();

    void Start()
    {
        foreach (var weaponPanel in weaponPanels)
        {
            var view = weaponPanel.GetComponent<WeaponViewModel>();
            var outline = weaponPanel.GetComponentInChildren<Outline>();
            view.SetImage(defaultImage);
            viewMap[view.type] = new WeaponPanel(view, outline);
        }
    }

    public void Init()
    {
        // Initされる前から所持している武器のViewを更新
        inventory.weapons.ToObservable().Subscribe(v => UpdateView(v.Key, v.Value.model.image));

        inventory.weapons.ObserveReplace().Subscribe(v => UpdateView(v.Key, v.NewValue.model.image));
        inventory.weapons.ObserveAdd().Subscribe(v => UpdateView(v.Key, v.Value.model.image));
        inventory.weapons.ObserveRemove().Subscribe(v => UpdateView(v.Key, defaultImage));

        this.ObserveEveryValueChanged(_ => inventory.currentWeaponType)
            .Subscribe(type =>
            {
                foreach (var weaponPanel in viewMap)
                {
                    if (weaponPanel.Key == type)
                        weaponPanel.Value.outline.enabled = true;
                    else
                        weaponPanel.Value.outline.enabled = false;
                }
            });
    }

    private void UpdateView(InventoryType type, Sprite image)
    {
        var view = viewMap[type];
        view.weaponVM.SetImage(image);
    }
}
