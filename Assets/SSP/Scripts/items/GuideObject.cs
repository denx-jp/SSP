using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GuideObject : MonoBehaviour
{
    public List<Guide> Guides = new List<Guide>();

    public virtual bool ShouldGuide()
    {
        return true;
    }

    public virtual Subject<Unit> GetHideGuideStream()
    {
        return new Subject<Unit>();
    }
}
