using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private Subject<bool> timeupStream = new Subject<bool>();
    [SerializeField, SyncVar(hook = "OnChangeCurrentTime")] private int currentTime = 0;

    public void Init()
    {
        if (isServer)
        {
            int limitTimeSec = limitMinutes * 60 + limitSeconds;
            currentTime = limitTimeSec;
            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(v => currentTime -= countDownSpeed).AddTo(this.gameObject);
        }
        timeStream.Where(time => time <= 0).Subscribe(_ => timeupStream.OnNext(true)).AddTo(this.gameObject);
    }

    public Subject<int> GetTimeStream()
    {
        return timeStream;
    }
    public Subject<bool> GetTimeupStream()
    {
        return timeupStream;
    }

    void OnChangeCurrentTime(int time)
    {
        timeStream.OnNext(time);
    }
}