using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class BulletManager : MonoBehaviour
{
    void Start()
    {
        Observable.Timer(TimeSpan.FromSeconds(model.deathTime))
            .Subscribe(_ => Destroy(this.gameObject))
            .AddTo(this.gameObject);
    }

    {
    }
}
