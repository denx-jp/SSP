using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.Networking;
using System.Linq;
using System;

public class TimeManager : NetworkBehaviour
{
    [SerializeField] private int limitMinutes;
    [SerializeField] private int limitSeconds;
    [SerializeField] private string resultSceneName;

    public Subject<int> timeStream = new Subject<int>();
    private Subject<bool> resultStream;
    [SerializeField,SyncVar(hook = "OnChangeCurrentTime")] private int currentTime = 0;

    void Start()
    {
        int limitTimeSec = limitMinutes * 60 + limitSeconds;
        if (isServer)
        {
            currentTime = limitTimeSec;
            var countdownClock = Observable.Interval(System.TimeSpan.FromSeconds(1)).TakeUntilDestroy(this.gameObject).Select(_ => (int)1);
            countdownClock.Subscribe(v => currentTime -= v);
        }

        resultStream = new Subject<bool>();

        timeStream
            .Where(time => time <= 0)
            .Subscribe(_ =>
            {
                resultStream.OnNext(true);
                SceneManager.LoadScene(resultSceneName);
            })
            .AddTo(this.gameObject);
    }

    public UniRx.IObservable<int> GetTimeStream()
    {
        return timeStream;
    }
    public Subject<bool> GetResultStream()
    {
        return resultStream;
    }

    void OnChangeCurrentTime(int time)
    {
        timeStream.OnNext(time);
    }
}