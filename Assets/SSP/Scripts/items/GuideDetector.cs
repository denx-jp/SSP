using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GuideDetector : MonoBehaviour
{
    public Subject<GuideObject> nearestGuideStream = new Subject<GuideObject>();

    public List<GuideObject> GuideObjects = new List<GuideObject>();
    public ReactiveDictionary<string, GuideObject> NearestGuideObjectMap = new ReactiveDictionary<string, GuideObject>();

    void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Select(col => col.GetComponent<GuideObject>())
            .Where(gObj => gObj != null)
            .Subscribe(guideObject =>
            {
                GuideObjects.Add(guideObject);

                var keyCode = guideObject.KeyCode;
                if (!NearestGuideObjectMap.ContainsKey(keyCode))
                    NearestGuideObjectMap[keyCode] = guideObject;
            });

        this.OnTriggerExitAsObservable()
            .Select(col => col.GetComponent<GuideObject>())
            .Where(gObj => gObj != null)
            .Subscribe(guideObject =>
            {
                GuideObjects.Remove(guideObject);

                var keyCode = guideObject.KeyCode;
                if (guideObject == NearestGuideObjectMap[keyCode])
                    NearestGuideObjectMap.Remove(keyCode);
            });

        this.UpdateAsObservable()
            .Where(_ => GuideObjects.Count > 1)     // GuideObjectsが1個以下の時は重複が起きないのでチェックしない。
            .Subscribe(_ =>
            {
                // キーが重複するGuideObjectがあるかチェック。
                var duplicateKey = GuideObjects.GroupBy(v => v.KeyCode).Where(v => v.Count() > 1).Select(v => v.Key).First();
                if (duplicateKey != null)
                {
                    NearestGuideObjectMap[duplicateKey] = GetNearestGuide(duplicateKey);
                }
            });
    }

    private GuideObject GetNearestGuide(string keyCode)
    {
        return GuideObjects.Where(v => v.KeyCode == keyCode).OrderBy(v => Vector3.Distance(v.transform.position, transform.position)).First();
    }
}
