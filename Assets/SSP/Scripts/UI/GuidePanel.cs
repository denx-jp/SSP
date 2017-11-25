using UnityEngine;
using UnityEngine.UI;

public class GuidePanel : MonoBehaviour
{
    public Text KeyText;
    public Text DescriptionText;

    public void Show(GuideObject guideObject)
    {
        KeyText.text = guideObject.KeyCode;
        DescriptionText.text = guideObject.Description;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        KeyText.text = string.Empty;
        DescriptionText.text = string.Empty;
        gameObject.SetActive(false);
    }
}
