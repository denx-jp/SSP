using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponModel : NetworkBehaviour
{
    [SerializeField, SyncVar] private int playerId, teamId;
    [SyncVar] public NetworkInstanceId parentId;

    void Start()
    {
        if (parentId != null)
            SetParent();
    }

    [ClientCallback]
    private void SetParent()
    {
        var player = ClientScene.FindLocalObject(parentId);
        if (player == null) return;
        transform.SetParent(player.GetComponent<PlayerInventoryManager>().rightHandTransform);
        transform.localPosition = Vector3.zero;
    }
}
