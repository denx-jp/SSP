using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RootMotion;
using RootMotion.FinalIK;
using UniRx;
using UniRx.Triggers;

public class CarriableObject : MonoBehaviour, IGuideable
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
    private bool canCarry = true;
    private NetworkTransform networkTransform;

    private Subject<Unit> hideGuideStream = new Subject<Unit>();

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();
        obj = GetComponent<InteractionObject>();
        networkTransform = GetComponent<NetworkTransform>();
        canCarry = true;
        defaultLayer = gameObject.layer;

        this.LateUpdateAsObservable()
            .Where(_ => interactionSystem != null)
            .Where(_ => interactionSystem.IsPaused(FullBodyBipedEffector.LeftHand))
            .Where(_ => holdWeight < 0.999)     //< 0.999を1とみなす
            .Subscribe(_ =>
            {
                holdWeight = Mathf.SmoothDamp(holdWeight, 1f, ref holdWeightVel, pickUpTime);

                transform.position = Vector3.Lerp(pickUpPosition, holdPoint.position, holdWeight);
                transform.rotation = Quaternion.Lerp(pickUpRotation, holdPoint.rotation, holdWeight);
            });
    }

    public virtual bool CanCarry()
    {
        return canCarry;
    }

    public void Pickup(InteractionSystem _interactionSystem, Transform _holdPoint)
    {
        canCarry = false;
        gameObject.layer = LayerMap.CarryObject;

        interactionSystem = _interactionSystem;
        holdPoint = _holdPoint;

        interactionSystem.OnInteractionStart += OnStart;
        interactionSystem.OnInteractionPause += OnPause;
        interactionSystem.OnInteractionResume += OnDrop;

        networkTransform.enabled = false;
        transform.parent = interactionSystem.transform;

        interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, obj, false);
        interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, obj, false);

        hideGuideStream.OnNext(Unit.Default);
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
        canCarry = true;
    }

    private void OnPause(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
    {
        if (effectorType != FullBodyBipedEffector.LeftHand) return;
        if (interactionObject != obj) return;

        pickUpPosition = transform.position;
        pickUpRotation = transform.rotation;
        holdWeight = 0f;
        holdWeightVel = 0f;
    }

    private void OnStart(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
    {
        if (effectorType != FullBodyBipedEffector.LeftHand) return;
        if (interactionObject != obj) return;

        rigid.isKinematic = true;

        // LSSをプレイヤー方向に向ける
        var groundDirectionFromPlayer = (this.transform.position - interactionSystem.transform.position).normalized;
        groundDirectionFromPlayer.y = 0;
        this.transform.forward = groundDirectionFromPlayer;

        #region Pivotの回転
        Vector3 characterDirection = (pivot.position - interactionSystem.transform.position).normalized;
        characterDirection.y = 0f;

        Vector3 characterDirectionLocal = obj.transform.InverseTransformDirection(characterDirection);

        Vector3 axis = QuaTools.GetAxis(characterDirectionLocal);
        Vector3 upAxis = QuaTools.GetAxis(obj.transform.InverseTransformDirection(interactionSystem.transform.up));

        pivot.localRotation = Quaternion.LookRotation(axis, upAxis);
        #endregion

        holdPoint.rotation = transform.rotation;
    }

    private void OnDrop(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
    {
        if (effectorType != FullBodyBipedEffector.LeftHand) return;
        if (interactionObject != obj) return;

        transform.parent = null;
        networkTransform.enabled = true;
        transform.transform.rotation = Quaternion.identity;
        rigid.isKinematic = false;
    }

    #region Guideまわり
    public bool ShouldGuide()
    {
        return CanCarry();
    }

    public Subject<Unit> GetHideGuideStream()
    {
        return hideGuideStream;
    }
    #endregion
}
