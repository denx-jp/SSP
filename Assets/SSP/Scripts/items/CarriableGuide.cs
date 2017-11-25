using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CarriableGuide : MonoBehaviour, IGuideable
{
    [SerializeField] CarriableLSS carriable;
    [SerializeField] List<Guide> Guides = new List<Guide>();
    private Subject<Unit> showGuideStream = new Subject<Unit>();
    private Subject<Unit> hideGuideStream = new Subject<Unit>();

    void Start()
    {
        GameManager.Instance.ConnectionPreparedStram
            .Subscribe(_ =>
            {
                // 運搬不可になったタイミングでガイドを非表示にするストリームを流す
                this.ObserveEveryValueChanged(__ => carriable.CanCarry())
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
        return carriable.CanCarry();
    }

    public Subject<Unit> GetShowGuideStream()
    {
        return showGuideStream;
    }

    public Subject<Unit> GetHideGuideStream()
    {
        return hideGuideStream;
    }
}
