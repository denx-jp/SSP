using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerOutlineManager : NetworkBehaviour
{
    public Color friendColor;
    public Color enemyColor;
    public SkinnedMeshRenderer[] renderers;

    private void Start()
    {
        GameManager.Instance.ConnectionPreparedStram
            .Subscribe(_ =>
            {
                var model = GetComponent<PlayerModel>();
                Debug.LogError("connect");
                if (model.isLocalPlayerCharacter) return;
                Debug.LogError("not local");

                var myTeamId = ClientPlayersManager.Instance.GetLocalPlayer().playerModel.teamId;
                if (model.teamId == myTeamId)
                {
                    SetOutlineColor(friendColor);
                }
                else
                {
                    SetOutlineColor(enemyColor);
                }
            });
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
