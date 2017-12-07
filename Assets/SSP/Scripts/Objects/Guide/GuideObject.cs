using UnityEngine;

public class GuideObject : MonoBehaviour
{
    public string KeyCode;
    public string Description;

    public virtual bool ShouldGuide()
    {
        return true;
    }
}
