using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerAvoider : MonoBehaviour
{
    private int avoidHash = Animator.StringToHash("RollForward");
    private Animator animator;
    private AnimatorStateInfo state;
    private PlayerInputManager pim;
    
    [SerializeField] private float avoidStartTime = 0.1f;
    [SerializeField] private float avoidDuration = 0.3f;

    private int syncLayer; //のちのちSyncVarに指定
    [SerializeField] private int DefaultLayer = 0;
    [SerializeField] private int InvincibleLayer = 9;

    private void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();

        pim.AvoidButtonDown
            .Where(v => v)
            .Where(_ => state.shortNameHash != avoidHash)
            .Subscribe(v =>
            {
                StartCoroutine(Avoiding());
            });
    }
    
    private void Update()
    {
        state = animator.GetCurrentAnimatorStateInfo(0);
        SyncLayer();
    }

   
    private IEnumerator Avoiding()
    {
        animator.SetTrigger(avoidHash);
        yield return new WaitForSeconds(avoidStartTime);
        CmdSetLayer(InvincibleLayer);
        yield return new WaitForSeconds(avoidDuration);
        CmdSetLayer(DefaultLayer);
    }

    //[Command]
    private void CmdSetLayer(int layer)
    {
        syncLayer = layer;
    }

    //[ClientCallback]
    private void SyncLayer()
    {
        this.gameObject.layer = syncLayer;
    }
}
