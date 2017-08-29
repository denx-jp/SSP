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

    void Start()
    private void Start()
    {
        animator = GetComponent<Animator>();
        pim = GetComponent<PlayerInputManager>();

        pim.AvoidButtonDown
            .Where(v => v)
            .Where(_ => state.shortNameHash != avoidHash)
            .Subscribe(v =>
            {
                animator.SetTrigger(avoidHash);
            });

    }
}
