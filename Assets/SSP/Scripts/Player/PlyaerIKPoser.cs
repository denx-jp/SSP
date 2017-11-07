using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(AimIK))]
[RequireComponent(typeof(FullBodyBipedIK))]
public class PlyaerIKPoser : MonoBehaviour
{
    private AimIK aim;
    private FullBodyBipedIK ik;

    [SerializeField, Range(0f, 1f)] private float headLookWeight = 1f;
    [SerializeField] private Vector3 gunHoldOffset;
    [SerializeField] private Vector3 leftHandOffset;
    [SerializeField] private Recoil recoil;

    private Vector3 headLookAxis;
    private Vector3 leftHandPosRelToRightHand;
    private Quaternion leftHandRotRelToRightHand;
    private Transform aimTarget;
    private Quaternion rightHandRotation;

    private void Start()
    {
        aim = GetComponent<AimIK>();
        ik = GetComponent<FullBodyBipedIK>();
        ik.solver.OnPreRead += OnPreRead;

        aim.enabled = false;
        ik.enabled = false;

        headLookAxis = ik.references.head.InverseTransformVector(ik.references.root.forward);
        Debug.Log(aim.solver.transform);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                // IK手続き、カメラが移動/回転した後にこれが更新されていることを確認する文字の現在のポーズから何かをサンプリングする
                Read();

                // AimIK pass
                AimIK();

                // FBBIKを通過させる - 左手を右手の相対位置に戻し、AimIKを解く。
                FBBIK();

                // Rotate the head to look at the aim target
                HeadLookAt(aimTarget.position);
            });
    }

    public void SetTarget(Transform _target)
    {
        aimTarget = _target;
    }

    private void Read()
    {
        // 右手に対する左手の位置と回転を記録する
        leftHandPosRelToRightHand = ik.references.rightHand.InverseTransformPoint(ik.references.leftHand.position);
        leftHandRotRelToRightHand = Quaternion.Inverse(ik.references.rightHand.rotation) * ik.references.leftHand.rotation;
    }

    private void AimIK()
    {
        // Set AimIK target position and update
        aim.solver.IKPosition = aimTarget.position;
        aim.solver.Update(); // Update AimIK
    }

    // 照準が終わった後、左手をガンに置く
    private void FBBIK()
    {
        // 右手の現在の回転を保存する
        rightHandRotation = ik.references.rightHand.rotation;

        // Offsetting hands, you might need that to support multiple weapons with the same aiming pose
        Vector3 rightHandOffset = ik.references.rightHand.rotation * gunHoldOffset;
        ik.solver.rightHandEffector.positionOffset += rightHandOffset;

        if (recoil != null) recoil.SetHandRotations(rightHandRotation * leftHandRotRelToRightHand, rightHandRotation);

        // Update FBBIK
        ik.solver.Update();

        // Rotating the hand bones after IK has finished
        if (recoil != null)
        {
            ik.references.rightHand.rotation = recoil.rotationOffset * rightHandRotation;
            ik.references.leftHand.rotation = recoil.rotationOffset * rightHandRotation * leftHandRotRelToRightHand;
        }
        else
        {
            ik.references.rightHand.rotation = rightHandRotation;
            ik.references.leftHand.rotation = rightHandRotation * leftHandRotRelToRightHand;
        }
    }

    // FBBIKが解決される前の最終的な計算。 リコイルはすでに解決されているので、計算されたオフセットを使用することができます。
    // ここでは、右手の位置と回転に関連して左手の位置を設定します。
    private void OnPreRead()
    {
        Quaternion r = recoil != null ? recoil.rotationOffset * rightHandRotation : rightHandRotation;
        Vector3 leftHandTarget = ik.references.rightHand.position + ik.solver.rightHandEffector.positionOffset + r * leftHandPosRelToRightHand;
        ik.solver.leftHandEffector.positionOffset += leftHandTarget - ik.references.leftHand.position - ik.solver.leftHandEffector.positionOffset + r * leftHandOffset;
    }

    // Rotating the head to look at the target
    private void HeadLookAt(Vector3 lookAtTarget)
    {
        Quaternion headRotationTarget = Quaternion.FromToRotation(ik.references.head.rotation * headLookAxis, lookAtTarget - ik.references.head.position);
        ik.references.head.rotation = Quaternion.Lerp(Quaternion.identity, headRotationTarget, headLookWeight) * ik.references.head.rotation;
    }

    // Cleaning up the delegates
    void OnDestroy()
    {
        if (ik != null) ik.solver.OnPreRead -= OnPreRead;
    }
}
