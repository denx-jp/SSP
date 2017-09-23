using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleUIManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private ClientPlayersManager clientPlayersManager;

    [SerializeField] private HealthViewModel healthViewModel;
    [SerializeField] private EtherViewModel etherViewModel;
    [SerializeField] private TimeViewModel timeViewModel;
    [SerializeField] private KillLogViewModel killLogViewModel;

    private void Start()
    {
        healthViewModel.healthModel = playerManager.playerModel as IHealth;
        etherViewModel.etherModel = playerManager.playerModel as IEther;
        killLogViewModel.SetKillLogNotifier(clientPlayersManager.GetPlayersComponent<PlayerKillLogNotifier>());
        timeViewModel.SetTimeManager(timeManager);
        
        //各VMに必要な代入がされる前に初期化処理をされると困るので明示的にタイミングを指定するためにInit()を使っている
        healthViewModel.Init();
        etherViewModel.Init();
        killLogViewModel.Init();
        timeViewModel.Init();
    }

    public void Init()
    {
        healthViewModel = GetComponentInChildren<HealthViewModel>();
        etherViewModel = GetComponentInChildren<EtherViewModel>();
        timeViewModel = GetComponentInChildren<TimeViewModel>();
        killLogViewModel = GetComponentInChildren<KillLogViewModel>();
    }

    public void SetPlayerManager(PlayerManager pm)
    {
        playerManager = pm;
    }
}
