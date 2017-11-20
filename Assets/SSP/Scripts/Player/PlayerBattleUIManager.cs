using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleUIManager : MonoBehaviour
{
    [SerializeField] private HealthViewModel healthViewModel;
    [SerializeField] private EtherViewModel etherViewModel;
    [SerializeField] private InventoryViewModel inventoryViewModel;
    [SerializeField] private EtherViewModel friendLssEtherViewModel;
    [SerializeField] private EtherViewModel enemyLssEtherViewModel;
    [SerializeField] private KillLogViewModel killLogViewModel;

    public void Init(PlayerManager pm, IEther friendLssModel, IEther enemyLssModel)
    {
        //各VMに必要な代入がされる前に初期化処理をされると困るので明示的にタイミングを指定するためにInit()を使っている
        healthViewModel.Init(pm.playerModel as IHealth);
        etherViewModel.Init(pm.playerModel as IEther);
        inventoryViewModel.Init(pm.playerInventory);
        friendLssEtherViewModel.Init(friendLssModel);
        enemyLssEtherViewModel.Init(enemyLssModel);
        killLogViewModel.Init(ClientPlayersManager.Instance.GetPlayersComponent<PlayerKillLogNotifier>());
    }
}
