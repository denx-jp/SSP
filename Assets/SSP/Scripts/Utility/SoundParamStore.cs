using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundParamStore : MonoBehaviour
{

    public static SoundParamStore Instance;

    public List<SoundParameter> soundParams = new List<SoundParameter>();

    private void Awake()
    {
        Instance = this;
    }

    public static SoundParameter GetSoundParam(GroundType groundType)
    {
        var candidates = Instance.soundParams.Where(v => v.groundType == groundType);
        if (candidates.Count() == 0) return GetSoundParam(GroundType.Dart);
        return candidates.First();
    }
}
