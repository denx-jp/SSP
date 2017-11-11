using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.FinalIK;
using UniRx;
using UniRx.Triggers;

public class CarriableObject : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private float pickUpTime = 0.3f;
    [HideInInspector] public InteractionObject obj;

    private Rigidbody rigid;
    private InteractionSystem interactionSystem;
    private Transform holdPoint;
    private float holdWeight, holdWeightVel;
    private Vector3 pickUpPosition;
    private Quaternion pickUpRotation;
    private int defaultLayer;

    public bool CanCarry = true;


    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        obj = GetComponent<InteractionObject>();
        CanCarry = true;
        defaultLayer = gameObject.layer;

        this.LateUpdateAsObservable()
            .Where(_ => !CanCarry)
            .Where(_ => holdWeight < 0.999)     //< 0.999を1とみなす
            .Subscribe(_ =>
            {
                // Smoothing in the hold weight
                holdWeight = Mathf.SmoothDamp(holdWeight, 1f, ref holdWeightVel, pickUpTime);

                // Interpolation
                transform.position = Vector3.Lerp(pickUpPosition, holdPoint.position, holdWeight);
                transform.rotation = Quaternion.Lerp(pickUpRotation, holdPoint.rotation, holdWeight);
            });
    }

    public void Pickup(InteractionSystem _interactionSystem, Transform _holdPoint)
    {
        CanCarry = false;
        gameObject.layer = LayerMap.CarryObject;

        interactionSystem = _interactionSystem;
        holdPoint = _holdPoint;

        // Listen to interaction events
        interactionSystem.OnInteractionStart += OnStart;
        interactionSystem.OnInteractionPause += OnPause;
        interactionSystem.OnInteractionResume += OnDrop;

        // オブジェクトがキャラクターの動きを継承するようにする
        transform.parent = interactionSystem.transform;

        interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, obj, false);
        interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, obj, false);
    }

    public void Drop()
    {
        interactionSystem.ResumeAll();

        interactionSystem.OnInteractionStart -= OnStart;
        interactionSystem.OnInteractionPause -= OnPause;
        interactionSystem.OnInteractionResume -= OnDrop;

        interactionSystem = null;
        holdPoint = null;

        gameObject.layer = defaultLayer;
        CanCarry = true;
    }

    // インタラクションが一時停止されたとき（トリガーの場合）、InteractionSystemによって呼び出されます。
    private void OnPause(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
    {
        if (effectorType != FullBodyBipedEffector.LeftHand) return;
        if (interactionObject != obj) return;

        // Make the object kinematic
        rigid.isKinematic = true;

        // Set object pick up position and rotation to current
        pickUpPosition = transform.position;
        pickUpRotation = transform.rotation;
        holdWeight = 0f;
        holdWeightVel = 0f;
    }

    // インタラクションの開始時にInteractionSystemによって呼び出されます。
    private void OnStart(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
    {
        if (effectorType != FullBodyBipedEffector.LeftHand) return;
        if (interactionObject != obj) return;

        // Rotate the pivot of the hand targets
        RotatePivot();

        // Rotate the hold point so it matches the current rotation of the object
        holdPoint.rotation = transform.rotation;
    }

    // 相互作用が再開されて一時停止されたときにInteractionSystemによって呼び出されます。
    private void OnDrop(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
    {
        if (effectorType != FullBodyBipedEffector.LeftHand) return;
        if (interactionObject != obj) return;

        transform.parent = null;
        rigid.isKinematic = false;
    }

    private void RotatePivot()
    {
        // キャラクターに向かって平らな方向を得る
        Vector3 characterDirection = (pivot.position - interactionSystem.transform.position).normalized;
        characterDirection.y = 0f;

        // オブジェクトのローカル空間に方向を変換する
        Vector3 characterDirectionLocal = obj.transform.InverseTransformDirection(characterDirection);

        // QuaTools.GetAxisは任意の方向に対して90度の正射投影軸を返します
        Vector3 axis = QuaTools.GetAxis(characterDirectionLocal);
        Vector3 upAxis = QuaTools.GetAxis(obj.transform.InverseTransformDirection(interactionSystem.transform.up));

        // Rotate towards axis and upAxis
        pivot.localRotation = Quaternion.LookRotation(axis, upAxis);
    }
}
