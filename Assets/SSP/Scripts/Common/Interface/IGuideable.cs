using System.Collections.Generic;
using UniRx;

public interface IGuideable
{
    bool ShouldGuide();
    Guide GetGuide();
}
