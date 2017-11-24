using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class GuideViewModel : MonoBehaviour
{
    [System.Serializable]
    private struct Guide
    {
        public GameObject Panel;
        public Text KeyText;
        public Text DescriptionText;
    }

    [SerializeField] private List<Guide> guides;

    private Dictionary<string, Guide> guideMap = new Dictionary<string, Guide>();
    private List<GuideObject> guideObjects = new List<GuideObject>();
    private Transform detectorTransform;

    public void Init(GuideDetector detector)
    {
        detectorTransform = detector.transform;

        detector.detectGuideStream
            .Subscribe(guideObject =>
            {
                guideObjects.Add(guideObject);

                if (guideMap.Count == 0)
                {
                    // 普通に表示
                }
                else
                {
                    // 重複チェック
                }
            });

        detector.missGuideStream
            .Subscribe(guideObject =>
            {
                guideObjects.Remove(guideObject);

                // guideObjectsにKeyの重複チェック
                // 重複があればそれを表示
            });

        this.UpdateAsObservable()
                .Where(_ => guideObjects.Count > 1)
                .Subscribe(_ =>
                {
                    foreach (var guideKeyValPair in guideMap)
                    {
                        //var keyUseGuideObjects = guideObjects.Where(v => v.Key == usedKey).ToList();
                        //if (keyUseGuideObjects.Count < 2) return;
                        //var nearestObject = keyUseGuideObjects.OrderBy(v => Vector3.Distance(v.transform.position, detectorTransform.position)).First();
                        //SetGuide(nearestObject);
                    }
                });
    }

    private void SetGuide(GuideObject guideObject)
    {
        var deactiveGuide = guides.Find(v => !v.Panel.activeSelf);
        deactiveGuide.KeyText.text = guideObject.Key;
        deactiveGuide.DescriptionText.text = guideObject.Description;
        deactiveGuide.Panel.SetActive(true);
        guideMap[guideObject.Key] = deactiveGuide;
    }

    private void DeactiveGuide(string key)
    {
        deactiveGuide.Panel.SetActive(true);
    }
}
