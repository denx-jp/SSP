using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TimeViewModel : MonoBehaviour {

    [SerializeField] private Text textTime;
    [SerializeField] private TimeManager timeManager;

    private string minutes, seconds;

    void Start() {
        timeManager
            .GetTimeStream()
            .Subscribe(time => {
                minutes = Mathf.Ceil(time / 60).ToString("00");
                seconds = Mathf.Ceil(time % 60).ToString("00");
                textTime.text = minutes + ":" + seconds;
            });
    }
}
