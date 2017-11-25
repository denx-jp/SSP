using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GuideViewModel : MonoBehaviour
{
    [SerializeField] private List<GuidePanel> guidePanels;
    private Dictionary<string, GuidePanel> guidePanelMap = new Dictionary<string, GuidePanel>();

    public void Init(GuideDetector detector)
    {
        detector.NearestGuideObjectMap.ObserveAdd()
            .Select(v => v.Value)
            .Subscribe(guide =>
            {
                var disabledGuide = guidePanels.Find(v => !v.gameObject.activeSelf);
                guidePanelMap[guide.KeyCode] = disabledGuide;
                disabledGuide.Show(guide);
            });

        detector.NearestGuideObjectMap.ObserveReplace()
         .Select(v => v.NewValue)
         .Subscribe(guide =>
         {
             guidePanelMap[guide.KeyCode].Show(guide);
         });

        detector.NearestGuideObjectMap.ObserveRemove()
            .Select(v => v.Value)
            .Subscribe(guideObject =>
            {
                guidePanelMap[guideObject.KeyCode].Hide();
            });
    }
}
