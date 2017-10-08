using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerInteractor : MonoBehaviour
{

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

        if (castResult.Length == 0) return;
        var interactionTargetObject = castResult
            .Where(v => v.gameObject.GetComponent<IInteractable>() != null)
            .Where(v => v.gameObject.GetComponent<IInteractable>().CanInteract())
            .OrderBy(v => Vector3.Distance(v.transform.position, this.transform.position));

        if (interactionTargetObject.Count() == 0) return;
        var interactionTarget = interactionTargetObject.First().GetComponent<IInteractable>();
        interactionTarget.Interact(playerManager);
    }
}
