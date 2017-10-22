using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private int limitMinutes;
    [SerializeField] private int limitSeconds;
    [SerializeField] private string resultSceneName;

    public IObservable<int> timeStream;
    private Subject<bool> resultStream;
    private int currentTime = 0;

    void Awake()
    {
        int limitTimeSec = limitMinutes * 60 + limitSeconds;

        timeStream = Observable.Interval(System.TimeSpan.FromSeconds(1))
              .Select(time => currentTime = limitTimeSec - (int)time)
              .TakeWhile(time => time >= 0)
              .Publish().RefCount(); ;
    }
    void Start()
    {
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
    public IObservable<int> GetTimeStream()
    {
        return timeStream;
    }
    public Subject<bool> GetResultStream()
    {
        return resultStream;
    }
}