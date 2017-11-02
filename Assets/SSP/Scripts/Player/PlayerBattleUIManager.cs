using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleUIManager : MonoBehaviour
{
    [SerializeField] private HealthViewModel healthViewModel;
    [SerializeField] private EtherViewModel etherViewModel;
    [SerializeField] private InventoryViewModel inventoryViewModel;
    [SerializeField] private EtherViewModel lssEtherViewModel;
    [SerializeField] private KillLogViewModel killLogViewModel;

    public void Init(PlayerManager pm, IEther lssModel, ClientPlayersManager cpm)
    {
        healthViewModel.healthModel = pm.playerModel as IHealth;
        etherViewModel.etherModel = pm.playerModel as IEther;
        inventoryViewModel.inventory = pm.playerInventory;
        lssEtherViewModel.etherModel = lssModel;
        killLogViewModel.SetKillLogNotifier(cpm.GetPlayersComponent<PlayerKillLogNotifier>());

        //各VMに必要な代入がされる前に初期化処理をされると困るので明示的にタイミングを指定するためにInit()を使っている
        healthViewModel.Init();
        etherViewModel.Init();
        inventoryViewModel.Init();
        lssEtherViewModel.Init();
        killLogViewModel.Init();
    }

    public void SetComponents()
    {
        healthViewModel = GetComponentInChildren<HealthViewModel>();
        etherViewModel = GetComponentInChildren<EtherViewModel>();
        killLogViewModel = GetComponentInChildren<KillLogViewModel>();
    }
}
