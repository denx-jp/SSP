using UniRx;

public interface IGuideable
{
    bool ShouldGuide();
    Subject<Unit> GetHideGuideStream();
}
