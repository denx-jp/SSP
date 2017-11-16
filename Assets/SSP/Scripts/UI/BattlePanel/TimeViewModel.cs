using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TimeViewModel : MonoBehaviour
{
    [SerializeField] private Text textTime;
    private TimeManager timeManager;

    private string minutes, seconds;

    public void Init()
    {
        timeManager
           .GetTimeStream()
           .Subscribe(time =>
           {
               minutes = Mathf.Ceil(time / 60).ToString("00");
               seconds = Mathf.Ceil(time % 60).ToString("00");
               textTime.text = minutes + ":" + seconds;
           });
    }

    public void SetTimeManager(TimeManager tm)
    {
        timeManager = tm;
    }
}
