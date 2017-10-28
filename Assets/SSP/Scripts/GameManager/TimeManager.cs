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
    private int countDownSpeed = 1;

    public Subject<int> timeStream = new Subject<int>();
    private Subject<bool> resultStream;
    [SerializeField, SyncVar(hook = "OnChangeCurrentTime")] private int currentTime = 0;

    public void Init()
    {
        int limitTimeSec = limitMinutes * 60 + limitSeconds;
        if (isServer)
        {
            currentTime = limitTimeSec;
            var countdownClock = Observable.Interval(System.TimeSpan.FromSeconds(1)).Subscribe(v => currentTime -= countDownSpeed).AddTo(this.gameObject);
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