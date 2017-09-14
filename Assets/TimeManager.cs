using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float limitMinutes;
    [SerializeField] private float limitSeconds;

    private ReactiveProperty<string> strTimeLimit;
    private UniRx.IObservable<long> timeStream;

    void Start()
    {

        float limitTimeSec = limitMinutes * 60 + limitSeconds;

        timeStream = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(time => time = (long)limitTimeSec - time)
            .TakeWhile(time => time >= 0);

        timeStream
            .Where(time => time <= 0)
            .Subscribe(_ => SceneManager.LoadScene(1))
            .AddTo(this.gameObject);

        strTimeLimit = timeStream
            .Select(time => ((int)Mathf.Ceil((time) / 60)).ToString() + ":" +
                                  ((int)Mathf.Ceil((time) % 60)).ToString())
            .ToReactiveProperty();

        strTimeLimit
            .Subscribe(v => Debug.Log(v));
    }
    public ReactiveProperty<string> GetCurrentTime()
    {
        return strTimeLimit;
    }
}