using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTransformSynchronizer : NetworkBehaviour
{
    [SyncVar] private Vector3 syncPosition;
    [SyncVar] private Quaternion syncRotation;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    [SerializeField] private float lerpRate = 15;
    [SerializeField] private float positionThreshold = 0.1f;
    [SerializeField] private float rotationThreshold = 1f;

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (Vector3.Distance(transform.position, lastPosition) > positionThreshold)
                TransmitPosition();
            if (Quaternion.Angle(transform.rotation, lastRotation) > rotationThreshold)
                TransmitRotation();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpRate);
        }
    }

    #region Position
    [ClientCallback]
    private void TransmitPosition()
    {
        CmdSyncPosition(transform.position);
        lastPosition = transform.position;
    }

    [Command]
    void CmdSyncPosition(Vector3 pos)
    {
        syncPosition = pos;
    }
    #endregion

    #region Rotation
    [ClientCallback]
    private void TransmitRotation()
    {
        CmdSyncRotation(transform.rotation);
        lastRotation = transform.rotation;
    }

    [Command]
    void CmdSyncRotation(Quaternion rotation)
    {
        syncRotation = rotation;
    }
    #endregion
}
