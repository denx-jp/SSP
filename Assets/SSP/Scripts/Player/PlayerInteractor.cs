using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerInteractor : MonoBehaviour {

    private Animator animator;
    private AnimatorStateInfo state;
    private PlayerInputManager pim;
    private PlayerManager playerManager;

    [SerializeField] private float interactableRadius;

    void Start()
    {
        pim = GetComponent<PlayerInputManager>();
        playerManager = GetComponent<PlayerManager>();

        pim.ActionButtonDown
            .Where(v => v)
            .Subscribe(v =>
            {
                Interact();
            });
    }

    void Interact()
    {
        var castResult = Physics.OverlapSphere(this.transform.position, interactableRadius);

        //検出されたオブジェクトのうち，IInteractableをもつもののうち最も近いものをターゲット
        if (castResult.Length == 0) return;
        var interactionTargetObject = castResult
            .Where(v => v.gameObject.GetComponent<IInteractable>() != null)
            .OrderBy(v => Vector3.Distance(v.transform.position, this.transform.position))
            .First();

        if (interactionTargetObject == null) return;
        var interactionTarget = interactionTargetObject.GetComponent<IInteractable>();
        interactionTarget.Interact(playerManager);
    }
}
