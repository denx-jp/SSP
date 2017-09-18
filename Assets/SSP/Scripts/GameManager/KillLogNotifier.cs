using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class KillLogNotifier : MonoBehaviour
{

    [SerializeField] private KillLogViewModel killLogVM;
    [SerializeField] private List<PlayerHealthManager> playerHealthManagers;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (var playerHealthManager in playerHealthManagers)
        {
            playerHealthManager.GetKillLogStream()
                .Subscribe(killLogInfo =>
                {
                    killLogVM.AppendKillLog(killLogInfo.Key.ToString(), killLogInfo.Value.ToString());
                });
        }
    }
}
