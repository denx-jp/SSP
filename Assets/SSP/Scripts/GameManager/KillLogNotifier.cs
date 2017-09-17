using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class KillLogNotifier : MonoBehaviour {

    [SerializeField] private KillLogViewModel killLogVM;
    [SerializeField] private PlayerHealthManager playerHealthManager;

    void Start(){
        playerHealthManager.GetKillLogStream()
                           .Subscribe(killLogInfo =>
                           {
                               killLogVM.AppendKillLog(killLogInfo.Key.ToString(), killLogInfo.Value.ToString());
                           });
    }
}
