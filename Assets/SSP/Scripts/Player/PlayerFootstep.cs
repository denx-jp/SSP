using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using System.Linq;

public enum FootstepType { Right, Left, Landing }

public class PlayerFootstep : NetworkBehaviour
{
    [SerializeField] private AudioClip footstep_Right;
    [SerializeField] private AudioClip footstep_Left;
    [SerializeField] private PlayerAnimationEventHandler animationEventHandler;
    [SerializeField] private AudioSource audioSource;
    private float castRadius = 0.1f;
    // Use this for initialization
    void Start()
    {
        animationEventHandler.FootstepStream.Subscribe(
            v =>
            {
                CmdPlayFootstep((int)v);
                if (isLocalPlayer)
                {
                    PlayFootstep(v);
                }
            }
        );
    }

    void PlayFootstep(FootstepType footstepType)
    {
        SetupAudioSourceParams(GetGroundType());
        Debug.Log(GetGroundType());
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
                audioSource.clip = footstep_Left;//一時的にLeft用の足音で代替
                audioSource.Play();
                break;
        }
    }

    GroundType GetGroundType()
    {
        //足元でスフィアキャストして地面の種類を判定
        var results = Physics.OverlapSphere(this.transform.position,0.1f);
        if (results.Length == 0) return GroundType.Air;
        var grounds = results.Select(v => v.gameObject).Where(v => v.GetComponent<GroundModel>() != null);
        if (grounds.Count() == 0) return GroundType.Air;
        return grounds.First().GetComponent<GroundModel>().groundType;
    }

    void SetupAudioSourceParams(GroundType groundType)
    {
        var param = SoundParamStore.GetSoundParam(groundType);
        //paramに各環境ごとのパラメータが入っているので必要なものをaudioSourceに代入してください
    }

    [Command]
    void CmdPlayFootstep(int footstepType)
    {
        RpcPlayFootstep(footstepType);
    }

    [ClientRpc]
    void RpcPlayFootstep(int footstepType)
    {
        if (isLocalPlayer) return;
        PlayFootstep((FootstepType)footstepType);
    }
}
