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

        public void Show(GuideObject guideObject)
        {
            KeyText.text = guideObject.KeyCode;
            DescriptionText.text = guideObject.Description;
            Panel.SetActive(true);
        }

        public void Hide()
        {
            KeyText.text = string.Empty;
            DescriptionText.text = string.Empty;
            Panel.SetActive(false);
        }
    }

    [SerializeField] private List<Guide> guides;

    private Dictionary<string, Guide> guideMap = new Dictionary<string, Guide>();

    public void Init(GuideDetector detector)
    {
        detector.NearestGuideObjectMap.ObserveAdd()
            .Select(v => v.Value)
            .Subscribe(guideObject =>
            {
                ShowGuide(guideObject);
            });

        detector.NearestGuideObjectMap.ObserveReplace()
         .Select(v => v.NewValue)
         .Subscribe(guideObject =>
         {
             UpdateGuide(guideObject);
         });

        detector.NearestGuideObjectMap.ObserveRemove()
            .Select(v => v.Value)
            .Subscribe(guideObject =>
            {
                guideMap[guideObject.KeyCode].Hide();
            });
    }

    private void ShowGuide(GuideObject guideObject)
    {
        var guide = GetDisabledGuide();
        guideMap[guideObject.KeyCode] = guide;
        guide.Show(guideObject);
    }

    private void UpdateGuide(GuideObject guideObject)
    {
        guideMap[guideObject.KeyCode].Show(guideObject);
    }

    private Guide GetDisabledGuide()
    {
        return guides.Find(v => !v.Panel.activeSelf);
    }
}
