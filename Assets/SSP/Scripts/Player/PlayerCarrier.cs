using UnityEngine;
using UniRx;
using System.Linq;
using UnityEngine.Networking;
using RootMotion.FinalIK;

public class PlayerCarrier : MonoBehaviour
{
    [SerializeField] private float searchRadius;
    [SerializeField] private Transform holdPoint;
    private PlayerInputManager pim;
    private InteractionSystem interactionSystem;

    private bool isHolding { get { return interactionSystem.IsPaused(FullBodyBipedEffector.LeftHand); } }
    private CarriableObject carriableObject;

    void Start()
    {
        interactionSystem = GetComponent<InteractionSystem>();
        pim = GetComponent<PlayerInputManager>();

        pim.Action2ButtonDown
            .Where(v => v)
            .Subscribe(v =>
            {
                if (!isHolding)
                    SearchCarriable();
                else
                    Drop();
            });

    }

    void SearchCarriable()
    {
        var castResult = Physics.OverlapSphere(this.transform.position, searchRadius);

        if (castResult.Length <= 0) return;

        var carriableObjects = castResult
            .Where(v => v.gameObject.GetComponent<CarriableObject>() != null)
            .Where(v => v.gameObject.GetComponent<CarriableObject>().CanCarry)
            .OrderBy(v => Vector3.Distance(v.transform.position, this.transform.position));

        if (carriableObjects.Count() <= 0) return;

        carriableObject = carriableObjects.First().gameObject.GetComponent<CarriableObject>();
        carriableObject.Pickup(interactionSystem, holdPoint);
    }

    void Drop()
    {
        carriableObject.Drop();
    }
}
