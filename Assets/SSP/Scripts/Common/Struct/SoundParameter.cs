using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundParameter
{
    public GroundType groundType;
    public bool bypassEffect;
    public bool bypassListenerEffects;
    public bool bypassReverbZones;
    public float dopplerLevel;
    public float maxDistance;
    public float minDistance;
    public float panStereo;
    public float pitch;
    public float reverbZoneMix;
    public float spatialBlend;
    public bool spatialize;
    public float spread;
    public float volume;
}
