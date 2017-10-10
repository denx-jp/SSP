using System.Linq;
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
        //Init();
    }

    public void Init()
    {
        healthViewModel.healthModel = PlayerManager().playerModel as IHealth;
        etherViewModel.etherModel = PlayerManager().playerModel as IEther;
        killLogViewModel.SetKillLogNotifier(ClientPlayersManager().GetPlayersComponent<PlayerKillLogNotifier>());
        timeViewModel.SetTimeManager(TimeManager());

        //各VMに必要な代入がされる前に初期化処理をされると困るので明示的にタイミングを指定するためにInit()を使っている
        healthViewModel.Init();
        etherViewModel.Init();
        killLogViewModel.Init();
        timeViewModel.Init();
    }

    public void SetComponents()
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

    private PlayerManager PlayerManager()
    {
        if (playerManager == null)
        {
            var localPlayer = GameObject.FindGameObjectsWithTag("Player").First(v => v.GetComponent<PlayerModel>().isLocalPlayerCharacter);
            playerManager = localPlayer.GetComponent<PlayerManager>();
        }
        return playerManager;
    }

    private TimeManager TimeManager()
    {
        if (timeManager == null)
        {
            var gameManager = GameObject.Find("GameManager");
            timeManager = gameManager.GetComponent<TimeManager>();
        }
        return timeManager;
    }

    private ClientPlayersManager ClientPlayersManager()
    {
        if (timeManager == null)
        {
            var gameManager = GameObject.Find("GameManager");
            clientPlayersManager = gameManager.GetComponent<ClientPlayersManager>();
        }
        return clientPlayersManager;
    }
}
