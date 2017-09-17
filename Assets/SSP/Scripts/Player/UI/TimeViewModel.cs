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

    // Use this for initialization
    void Start() {
        timeManager = gameManager.GetComponent<TimeManager>();

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                strTime = timeManager.GetCurrentTime();
                strTime.Subscribe(v => textTime.text = v);
            });
    }
}
