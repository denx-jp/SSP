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

    public ReactiveProperty<string> strTimeLimit;

    public UniRx.IObservable<long> timeStream;
    public Subject<bool> resultStream;

    void Start()
    {
        resultStream = new Subject<bool>();

        float limitTimeSec = limitMinutes * 60 + limitSeconds;

        timeStream = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(time => time = (long)limitTimeSec - time)
            .TakeWhile(time => time >= 0);

        strTimeLimit = timeStream
            .Select(time => ((int)Mathf.Ceil((time) / 60)).ToString() + ":" +
                                  ((int)Mathf.Ceil((time) % 60)).ToString())
            .ToReactiveProperty();

        timeStream
            .Where(time => time <= 0)
            .Subscribe(_ => 
            {
                resultStream.OnNext(true);
                SceneManager.LoadScene("NB29979-Result");
            })
            .AddTo(this.gameObject);
    }
    public ReactiveProperty<string> GetCurrentTime()
    {
        if (strTimeLimit == null)
        {
            return new ReactiveProperty<string>("99:59");
        }
        return strTimeLimit;
    }
}