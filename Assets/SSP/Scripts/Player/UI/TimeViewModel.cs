using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TimeViewModel : MonoBehaviour {

    [SerializeField] private Text textTime;
    [SerializeField] private GameObject gameManager;

    private TimeManager timeManager;

    void Start() {
        timeManager = gameManager.GetComponent<TimeManager>();

        timeManager
            .GetTimeStream()
            .Select(time => ((int)Mathf.Ceil((time) / 60)).ToString() + ":" +
                                  ((int)Mathf.Ceil((time) % 60)).ToString())
            .Subscribe(time => textTime.text = time.ToString());
    }
}
