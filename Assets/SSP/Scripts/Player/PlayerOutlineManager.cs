using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerOutlineManager : NetworkBehaviour {

    public Color friendColor;
    public Color enemyColor;

    public SkinnedMeshRenderer[] renderers;

    public void Start()
    {
        var model = GetComponentInParent<PlayerModel>();
        this.Init(model);
    }

    public void Init(PlayerModel model)
    {
        var teamId = model.teamId;
        var myTeamId = ClientPlayersManager.Instance.GetLocalPlayer().playerModel.teamId;
        if (teamId == myTeamId)
        {
            SetOutlineColor(friendColor);
        }
        else
        {
            SetOutlineColor(enemyColor);
        }
    }

    public void SetOutlineColor(Color color)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < renderers[i].materials.Length; j++)
            {
                renderers[i].materials[j].SetColor("_RimColor", color);
            }
        }
    }
}
