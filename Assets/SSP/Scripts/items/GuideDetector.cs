using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GuideDetector : MonoBehaviour
{
    public ReactiveDictionary<string, Guide> NearestGuideObjectMap = new ReactiveDictionary<string, Guide>();
    public List<Guide> Guides = new List<Guide>();

    void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Select(col => col.GetComponent<IGuideable>())
            .Where(guideObject => guideObject != null && guideObject.ShouldGuide())
            .Subscribe(guideObject =>
            {
                foreach (var guide in guideObject.GetGuides())
                {
                    guideObject.GetHideGuideStream()
                        .Subscribe(_ =>
                        {
                            RemoveGuide(guide);
                        });

                    guideObject.GetShowGuideStream()
                        .Subscribe(_ =>
                        {
                            Guides.Add(guide);
                            var k = guide.KeyCode;
                            if (!NearestGuideObjectMap.ContainsKey(k))
                                NearestGuideObjectMap[k] = guide;
                        });

                    Guides.Add(guide);
                    var keyCode = guide.KeyCode;
                    if (!NearestGuideObjectMap.ContainsKey(keyCode))
                        NearestGuideObjectMap[keyCode] = guide;
                }
            });

        this.OnTriggerExitAsObservable()
            .Select(col => col.GetComponent<IGuideable>())
            .Where(gObj => gObj != null)
            .Subscribe(guideObject =>
            {
                foreach (var guide in guideObject.GetGuides())
                    RemoveGuide(guide);
            });

        this.UpdateAsObservable()
            .Where(_ => Guides.Count > 1)     // GuideObjectsが1個以下の時は重複が起きないのでチェックしない。
            .Subscribe(_ =>
            {
                // キーが重複するGuideがあるかチェック。
                var duplicateKey = Guides.GroupBy(v => v.KeyCode).Where(v => v.Count() > 1);
                if (duplicateKey.Count() > 0)
                {
                    var key = duplicateKey.Select(v => v.Key).First();
                    NearestGuideObjectMap[key] = GetNearestGuide(key);
                }
            });
    }

    private Guide GetNearestGuide(string keyCode)
    {
        return Guides.Where(v => v.KeyCode == keyCode).OrderBy(v => Vector3.Distance(v.transform.position, transform.position)).First();
    }

    private void RemoveGuide(Guide guide)
    {
        Guides.Remove(guide);

        var keyCode = guide.KeyCode;
        if (NearestGuideObjectMap.ContainsKey(keyCode) && guide == NearestGuideObjectMap[keyCode])
            NearestGuideObjectMap.Remove(keyCode);
    }
}
