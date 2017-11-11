using UnityEngine;
using UniRx;
using System.Linq;
using UnityEngine.Networking;
using RootMotion.FinalIK;
using UniRx;
using UniRx.Triggers;

public class PlayerCarrier : MonoBehaviour
{
    [SerializeField] private float searchRadius;
    [SerializeField] private Transform holdPoint;
    private PlayerInputManager pim;
    private FullBodyBipedIK ik;
    private InteractionSystem interactionSystem;

    private bool canCarry = true;
    private CarriableObject carriableObject;

    void Start()
    {
        ik = GetComponent<FullBodyBipedIK>();
        interactionSystem = GetComponent<InteractionSystem>();
        pim = GetComponent<PlayerInputManager>();

        pim.Action2ButtonDown
            .Where(v => v)
            .Subscribe(v =>
            {
                if (canCarry)
                {
                    SearchCarriable();
                    canCarry = false;
                }
                else
                {
                    Drop();
                    canCarry = true;
                }
            });

        this.LateUpdateAsObservable()
            .Where(_ => !canCarry)
            .Subscribe(_ =>
            {
                ik.solver.Update();
            });
    }

    void SearchCarriable()
    {
        var castResult = Physics.OverlapSphere(this.transform.position, searchRadius);

        if (castResult.Length <= 0) return;

        var carriableObjects = castResult
            .Select(v => v.GetComponent<CarriableObject>())
            .Where(v => v != null)
            .Where(v => v.CanCarry)
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
