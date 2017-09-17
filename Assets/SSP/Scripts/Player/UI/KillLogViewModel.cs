using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class KillLogViewModel : MonoBehaviour
{
    [SerializeField] private int showPeriod = 3;
    [SerializeField] private List<Text> texts;

    private void Start()
    {
        foreach (Text text in texts)
        {
            text.text = "";
        }
    }

    public void AppendKillLog(string killer, string killed)
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
