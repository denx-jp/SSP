using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using RootMotion.FinalIK;
using UniRx;
using UniRx.Triggers;

public class PlayerCarrier : MonoBehaviour
{
    [SerializeField] private Vector3 boxCastCenterOffset;
    [SerializeField] private Vector3 boxCastHalfExtents;
    [SerializeField] private Transform holdPoint;

    private PlayerModel model;
    private PlayerInputManager pim;
    private FullBodyBipedIK ik;
    private InteractionSystem interactionSystem;

    private bool canCarry = true;
    private CarriableObject carriableObject;

    void Start()
    {
        ik = GetComponent<FullBodyBipedIK>();
        interactionSystem = GetComponent<InteractionSystem>();
        model = GetComponent<PlayerModel>();
        pim = GetComponent<PlayerInputManager>();

        pim.Action2ButtonDown
            .Where(v => v)
            .Where(_ => model.MoveMode != MoveMode.battle)
            .Subscribe(v =>
            {
                if (canCarry)
                {
                    SearchCarriable();
                }
                else
                {
                    Drop();
                    canCarry = true;
                    model.MoveMode = MoveMode.normal;
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
        var castResult = Physics.OverlapBox(transform.position + boxCastCenterOffset, boxCastHalfExtents, transform.rotation);

        if (castResult.Length <= 0) return;

        var carriableObjects = castResult
            .Select(v => v.GetComponent<CarriableObject>())
            .Where(v => v != null)
            .Where(v => v.CanCarry)
            .OrderBy(v => Vector3.Distance(v.transform.position, this.transform.position));

        if (carriableObjects.Count() <= 0) return;

        carriableObject = carriableObjects.First().gameObject.GetComponent<CarriableObject>();
        carriableObject.Pickup(interactionSystem, holdPoint);
        canCarry = false;
        model.MoveMode = MoveMode.carry;
    }

    void Drop()
    {
        carriableObject.Drop();
    }
}
