using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class KillLogNotifier : MonoBehaviour {

    private KillLogViewModel killLogVM;
    private PlayerHealthManager playerHealthManager;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject killLog;

    void Start(){
        playerHealthManager = player.GetComponent<PlayerHealthManager>();
        killLogVM = killLog.GetComponent<KillLogViewModel>();

        playerHealthManager.GetKillLogStream()
                           .Subscribe(killLogInfo =>
                           {
                               killLogVM.AppendKillLog(killLogInfo.Key.ToString(), killLogInfo.Value.ToString());
                           });
    }
}
