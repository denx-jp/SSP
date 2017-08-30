using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerHealthModel : HealthModel
{
    private int deathHash = Animator.StringToHash("Death");
    Animator animator;


    private void Start()
    {
        Init();

        animator = GetComponent<Animator>();

        this.deathStream
            .Subscribe(isdeath =>
            {
                animator.SetBool(deathHash, isdeath);
            });
    }

    public bool IsAlive()
    {
        return Health.Value > 0.0f;
    }

}
