using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GuideDetector : MonoBehaviour
{
    public Subject<GuideObject> detectGuideStream = new Subject<GuideObject>();
    public Subject<GuideObject> missGuideStream = new Subject<GuideObject>();

    void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Select(col => col.GetComponent<GuideObject>())
            .Where(gObj => gObj != null)
            .Subscribe(guideObject =>
            {
                detectGuideStream.OnNext(guideObject);
            });

        this.OnTriggerExitAsObservable()
            .Select(col => col.GetComponent<GuideObject>())
            .Where(gObj => gObj != null)
            .Subscribe(guideObject =>
            {
                missGuideStream.OnNext(guideObject);
            });
    }
}
