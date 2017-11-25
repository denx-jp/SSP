using System.Collections.Generic;
using UniRx;

public interface IGuideable
{
    bool ShouldGuide();
    List<Guide> GetGuides();
    Subject<Unit> GetShowGuideStream();
    Subject<Unit> GetHideGuideStream();
}
