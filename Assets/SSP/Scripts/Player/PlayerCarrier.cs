using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using RootMotion.FinalIK;
using UniRx;
using UniRx.Triggers;

public class PlayerCarrier : NetworkBehaviour
{
    [SerializeField] private Vector3 castCenterOffset;
    [SerializeField] private float castRadius;
    [SerializeField] private Transform holdPoint;

    private PlayerModel model;
    private PlayerHealthManager healthManager;
    private PlayerInputManager pim;
    private FullBodyBipedIK ik;
    private InteractionSystem interactionSystem;

    [SyncVar] private bool canCarry = true;
    private CarriableObject carriableObject;

    void Start()
    {
        ik = GetComponent<FullBodyBipedIK>();
        interactionSystem = GetComponent<InteractionSystem>();
        model = GetComponent<PlayerModel>();
        healthManager = GetComponent<PlayerHealthManager>();
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
                    if (carriableObject == null) return;
                    CmdDrop();
                }
            });

        this.LateUpdateAsObservable()
            .Where(_ => !canCarry)
            .Subscribe(_ =>
            {
                ik.solver.Update();
            });

        // 運搬中に死亡したらを手放す
        healthManager.GetDeathStream()
            .Where(_ => !canCarry && carriableObject != null)
            .Subscribe(_ =>
            {
                CmdDrop();
            });
    }

    void SearchCarriable()
    {
        // boxCastCenterOffsetをプレイヤーのローカル空間におけるオフセットに変換
        var offset = transform.right * castCenterOffset.x + transform.up * castCenterOffset.y + transform.forward * castCenterOffset.z;
        var castResult = Physics.OverlapSphere(transform.position + offset, castRadius);

        if (castResult.Length <= 0) return;

        var carriableObjects = castResult
            .Select(v => v.GetComponent<CarriableObject>())
            .Where(v => v != null)
            .Where(v => v.CanCarry());

        if (carriableObjects.Count() <= 0) return;

        var firstObject = carriableObjects
            .OrderBy(v => Vector3.Distance(v.transform.position, this.transform.position))
            .First().gameObject.GetComponent<CarriableObject>();

        CmdCarry(firstObject.gameObject);
    }

    [Command]
    void CmdCarry(GameObject go)
    {
        model.MoveMode = MoveMode.carry;
        canCarry = false;
        RpcCarry(go);
    }

    [ClientRpc]
    void RpcCarry(GameObject go)
    {
        carriableObject = go.GetComponent<CarriableObject>();
        carriableObject.Pickup(interactionSystem, holdPoint);
    }

    [Command]
    void CmdDrop()
    {
        canCarry = true;
        model.MoveMode = MoveMode.normal;
        RpcDrop();
    }

    [ClientRpc]
    void RpcDrop()
    {
        carriableObject.Drop();
    }
}
