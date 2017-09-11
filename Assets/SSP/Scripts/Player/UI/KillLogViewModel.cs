using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class KillLogViewModel : MonoBehaviour
{
    const int KILL_LOG_SHOW_PERIOD = 3;
    [SerializeField] private Text[] texts;

    public void AppendKillLog(string winner, string loser)
    {
        StartCoroutine(KillLogCoroutine(winner + " が " + loser + " を キル しました"));
    }

    private IEnumerator KillLogCoroutine(string killLogText)
    {
        var text = texts.First(v => v.text == "");
        text.text = killLogText;
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(KILL_LOG_SHOW_PERIOD);
        text.gameObject.SetActive(false);
        text.text = "";
    }
}
