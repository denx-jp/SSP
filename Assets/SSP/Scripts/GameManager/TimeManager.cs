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

    public Subject<int> timeStream;
    private Subject<bool> resultStream;
    [SerializeField] private int currentTime = 0;

    void Awake()
    {

    }

    void Start()
    {
        int limitTimeSec = limitMinutes * 60 + limitSeconds;
        timeStream = new Subject<int>();
        if (isServer)
        {
            currentTime = limitTimeSec;
            var countdownClock = Observable.Interval(System.TimeSpan.FromSeconds(1)).Select(_ => (int)1).Publish().RefCount();
            countdownClock.Subscribe(v => { currentTime -= v; timeStream.OnNext(currentTime); });
            timeStream.Subscribe(v => CmdTimeChange(v));
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

    [Command]
    void CmdTimeChange(int time)
    {
        RpcTimeSync(time);
    }

    [ClientRpc]
    void RpcTimeSync(int time)
    {
        if (isClient && !isServer)
        {
            timeStream.OnNext(time);
        }
    }
}