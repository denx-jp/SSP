using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GuideDetector : MonoBehaviour
{
    public Subject<GuideObject> guideDetectStream = new Subject<GuideObject>();

    void Start()
    {
        this.OnTriggerStayAsObservable()
            .Select(col => col.GetComponent<GuideObject>())
            .Where(gObj => gObj != null)
            .Subscribe(guideObject =>
            {
                guideDetectStream.OnNext(guideObject);
            });
    }
}
