using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using System.Linq;

public enum FootstepType { Right, Left, Landing }

public class PlayerFootstep : MonoBehaviour
{
    [SerializeField] private AudioClip footstep_Right;
    [SerializeField] private AudioClip footstep_Left;
    [SerializeField] private AudioClip footstep_Landing;
    [SerializeField] private PlayerAnimationEventHandler animationEventHandler;
    [SerializeField] private AudioSource audioSource;
    private float castRadius = 0.1f;
    void Start()
    {
        animationEventHandler.FootstepStream.Subscribe(
            v =>
            {
                PlayFootstep(v);
            }
        );
    }

    void PlayFootstep(FootstepType footstepType)
    {
        var groundType = GetGroundType();
        if (groundType == GroundType.Air) return;

        var soundParam = SoundParamStore.GetSoundParam(groundType);
        SetupAudioSourceParams(soundParam);
        
        switch (footstepType)
        {
            case FootstepType.Left:
                audioSource.clip = footstep_Left;
                audioSource.Play();
                break;
            case FootstepType.Right:
                audioSource.clip = footstep_Right;
                audioSource.Play();
                break;
            case FootstepType.Landing:
                audioSource.clip = footstep_Landing;//一時的にLeft用の足音で代替
                audioSource.Play();
                break;
        }
    }

    GroundType GetGroundType()
    {
        //足元でスフィアキャストして地面の種類を判定
        var results = Physics.OverlapSphere(this.transform.position,0.2f);
        if (results.Length == 0) return GroundType.Air;
        var grounds = results.Select(v => v.gameObject).Where(v => v.GetComponent<GroundModel>() != null);
        if (grounds.Count() == 0) return GroundType.Air;
        return grounds.First().GetComponent<GroundModel>().groundType;
    }

    void SetupAudioSourceParams(SoundParameter soundParam)
    {
        //paramに各環境ごとのパラメータが入っているので必要なものをaudioSourceに代入してください
    }
}
