using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InventoriableGuide : MonoBehaviour, IGuideable
{
    [SerializeField] InventoriableObject interactable;
    [SerializeField] List<Guide> Guides = new List<Guide>();
    private Subject<Unit> showGuideStream = new Subject<Unit>();
    private Subject<Unit> hideGuideStream = new Subject<Unit>();

    void Start()
    {
        this.ObserveEveryValueChanged(__ => interactable.canInteract)
            .Subscribe(can =>
            {
                if (can)
                {
                    showGuideStream.OnNext(Unit.Default);
                }
                else
                {
                    hideGuideStream.OnNext(Unit.Default);
                }
            });

        foreach (var guide in Guides)
        {
            guide.transform = transform;
        }
    }

    public List<Guide> GetGuides()
    {
        return Guides;
    }

    public virtual bool ShouldGuide()
    {
        return interactable.CanInteract();
    }

    public Subject<Unit> GetShowGuideStream()
    {
        return showGuideStream;
    }

    public virtual Subject<Unit> GetHideGuideStream()
    {
        return hideGuideStream;
    }
}
