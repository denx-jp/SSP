using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public enum FootstepType { R, L }

public class PlayerFootstep : NetworkBehaviour
{
    [SerializeField] List<AudioClip> footstepList = new List<AudioClip>();
    [SerializeField] private PlayerAnimationEventHandler animationEventHandler;
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
