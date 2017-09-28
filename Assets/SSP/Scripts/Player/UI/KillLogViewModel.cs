using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using UnityEngine.Networking;

public class KillLogViewModel : NetworkBehaviour
{
    [SerializeField] private List<PlayerKillLogNotifier> killLogNotifiers;
    [SerializeField] private List<Text> texts;
    [SerializeField] private int showPeriod = 3;

    public void Init()
    {
        foreach (Text text in texts)
            text.text = "";
        // Test
        CmdAppendKillLog("kobusi","21sai");
        foreach (var killLogNotifier in killLogNotifiers)
        {
            killLogNotifier.GetKillLogStream()
                .Subscribe(killLogInfo => CmdAppendKillLog(killLogInfo.Key.ToString(), killLogInfo.Value.ToString()));
        }
    }

    public void SetKillLogNotifier(List<PlayerKillLogNotifier> pklns)
    {
        killLogNotifiers = pklns;
    }

    [Command]
    private void CmdAppendKillLog(string killer, string killed)
    {
        RpcAppendKillLog(killer, killed);
        //Debug.Log("cmd");
    }

    [ClientRpc]
    private void RpcAppendKillLog(string killer, string killed)
    {
        StartCoroutine(KillLogCoroutine("プレイヤー" + killer + " が プレイヤー" + killed + " を キル しました"));
        //Debug.Log("rpc");
    }

    private IEnumerator KillLogCoroutine(string killLogText)
    {
        var text = texts.First(v => v.text == "");
        text.text = killLogText;
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(showPeriod);
        text.gameObject.SetActive(false);
        text.text = "";
    }

    [ContextMenu("Set Texts")]
    private void SetTexts()
    {
        var textComponents = this.GetComponentsInChildren<Text>();
        Debug.Log(textComponents.Count());
        foreach (var text in textComponents)
        {
            texts.Add(text);
        }
    }
}
