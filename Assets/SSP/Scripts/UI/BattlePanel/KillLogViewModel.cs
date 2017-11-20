using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class KillLogViewModel : MonoBehaviour
{
    [SerializeField] private List<PlayerKillLogNotifier> killLogNotifiers;
    [SerializeField] private List<Text> texts;
    [SerializeField] private int showPeriod = 3;

    public void Init(List<PlayerKillLogNotifier> pklns)
    {
        killLogNotifiers = pklns;

        foreach (Text text in texts)
            text.text = "";

        foreach (var killLogNotifier in killLogNotifiers)
        {
            killLogNotifier.GetKillLogStream()
                .Subscribe(killLogInfo => AppendKillLog(killLogInfo.Key.ToString(), killLogInfo.Value.ToString()));
        }
    }

    private void AppendKillLog(string killer, string killed)
    {
        StartCoroutine(KillLogCoroutine("プレイヤー" + killer + " が プレイヤー" + killed + " を キル しました"));
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
