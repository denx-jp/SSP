using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerGuideDetector : MonoBehaviour
{

    [SerializeField] private GameObject detector;

    private void Start()
    {
        detector.OnTriggerEnterAsObservable()
            .Where(v => v.tag == TagMap.GuideObject)
            .Subscribe(_ =>
            {

            });
    }
}
