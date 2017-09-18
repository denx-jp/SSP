using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleUIManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TimeManager timeManager;

    [SerializeField] private HealthViewModel healthViewModel;
    [SerializeField] private EtherViewModel etherViewModel;
    [SerializeField] private TimeViewModel timeViewModel;
    [SerializeField] private KillLogViewModel killLogViewModel;

    private void Awake()
    {
        healthViewModel.healthModel = playerManager.playerModel as IHealth;
        etherViewModel.etherModel = playerManager.playerModel as IEther;
        timeViewModel.SetTimeManager(timeManager);
        //killLogViewModel =;

        healthViewModel.Init();
        etherViewModel.Init();
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
