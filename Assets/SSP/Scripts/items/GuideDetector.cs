using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GuideDetector : MonoBehaviour
{
    [SerializeField] float castRadius = 2.0f;
    public ReactiveDictionary<string, GuideObject> NearestGuideObjectMap = new ReactiveDictionary<string, GuideObject>();
    public List<Guide> Guides = new List<Guide>();

    void Start()
    {
        var model = GetComponentInParent<PlayerModel>();

        this.UpdateAsObservable()
            .Where(_ => model.isLocalPlayerCharacter)
            .Subscribe(_ =>
            {
                var guideObjects = Physics.OverlapSphere(this.transform.position, castRadius, LayerMap.GuideObjectMask)
                        .Select(col => col.GetComponents<GuideObject>())
                        .SelectMany(v => v)
                        .Where(guideObject => guideObject != null && guideObject.ShouldGuide());

                if (guideObjects.Count() == 0)
                {
                    if (NearestGuideObjectMap.Count > 0)
                        NearestGuideObjectMap.Clear();
                    return;
                }

                var usedKeys = NearestGuideObjectMap.Select(v => v.Key).ToArray();
                foreach (var key in usedKeys)
                {
                    if (!guideObjects.Any(v => v.KeyCode == key))
                        NearestGuideObjectMap.Remove(key);
                }

                var groups = guideObjects.GroupBy(v => v.KeyCode);
                foreach (var groupEachKey in groups)
                {
                    var nearestGuide = groupEachKey.OrderBy(v => Vector3.Distance(v.transform.position, transform.position)).First();
                    NearestGuideObjectMap[groupEachKey.Key] = nearestGuide;
                }
            });
    }
}
