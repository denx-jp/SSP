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

        // キルログ表示用テキストを空にする
        foreach (Text text in texts)
            text.text = "";

        foreach (var killLogNotifier in killLogNotifiers)
        {
            killLogNotifier.GetKillLogStream()
                .Subscribe(killLogInfo => AppendKillLog(killLogInfo.Key, killLogInfo.Value));
        }
    }

    private void AppendKillLog(int killerId, int killedId)
    {
        var killer = ClientPlayersManager.Players[killedId];
        var killed = ClientPlayersManager.Players[killerId];
        var text = $"{killer.playerModel.Name}が{killed.playerModel.Name}をキル";
        StartCoroutine(KillLogCoroutine(text));
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
}
