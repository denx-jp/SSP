using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TimeViewModel : MonoBehaviour {

    [SerializeField] private Text textTime;
    [SerializeField] private GameObject gameManager;

    ReactiveProperty<string> strTime;
    private TimeManager timeManager;

    private UniRx.IObservable<long> timeStream;

    void Start() {
        timeManager = gameManager.GetComponent<TimeManager>();

        //timeManager.timeStream.OnErrorRetry().Subscribe(time => Debug.Log(time));
        //timeManager.resultStream.Subscribe(re => Debug.Log(re));
        //timeManager.GetTimeStream().Subscribe(v => Debug.Log(v));

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                strTime = timeManager.GetCurrentTime();
                strTime.Subscribe(v => textTime.text = v);
            });
    }
}
