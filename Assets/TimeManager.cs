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

    public IConnectableObservable<long> timeStream;
    private Subject<bool> resultStream;

    void Awake()
    {
        float limitTimeSec = limitMinutes * 60 + limitSeconds;

        timeStream = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(time => time = (long)limitTimeSec - time)
            .TakeWhile(time => time >= 0)
            .Publish();
    }
    void Start()
    {
        resultStream = new Subject<bool>();

        timeStream.Connect();

        timeStream
            .Where(time => time <= 0)
            .Subscribe(_ => 
            {
                resultStream.OnNext(true);
                SceneManager.LoadScene("NB29979-Result");
            })
            .AddTo(this.gameObject);
    }
    public IConnectableObservable<long> GetTimeStream()
    {
        return timeStream;
    }
    public Subject<bool> GetResultStream()
    {
        return resultStream;
    }
}