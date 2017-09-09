using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class KillLogViewModel : MonoBehaviour {

    [SerializeField] private Text textUI;

    /// <summary>
    /// キルログにログを追記します
    /// </summary>
    /// <param name="logText">追記する文字列</param>
    private void appendLogText(string logText)
    {
        if (string.IsNullOrEmpty(logText))
        {
            return;
        }
        textUI.text += logText + "\n";
    }

    /// <summary>
    /// キルしたときに表示されるキルログを設定します
    /// </summary>
    /// <param name="winner">キルしたプレイヤー</param>
    /// <param name="loser">キルされたプレイヤー</param>
    private void playerKillLog(string winner, string loser)
    {
        appendLogText(winner + " が " + loser + " を キル しました");
    }
}
