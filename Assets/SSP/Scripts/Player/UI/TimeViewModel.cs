using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TimeViewModel : MonoBehaviour {

    [SerializeField] private Text textTime;
    [SerializeField] private GameObject gameManager;

    private TimeManager timeManager;
    private string minutes, seconds;

    void Start() {
        timeManager = gameManager.GetComponent<TimeManager>();

        timeManager
            .GetTimeStream()
            .Subscribe(time => {
                minutes = TimeTextFormat(Mathf.Ceil(time / 60));
                seconds = TimeTextFormat(Mathf.Ceil(time % 60));
                textTime.text = minutes + ":" + seconds;
            });
    }
    private string TimeTextFormat(float time)
    {
        return time.ToString().Length == 2 ? 
            time.ToString() : "0" + time.ToString();
    }
}
