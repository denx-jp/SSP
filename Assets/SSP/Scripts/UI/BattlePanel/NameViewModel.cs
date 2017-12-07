using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class NameViewModel : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Color friendTeamColor;
    [SerializeField] private Color enemyTeamColor;

    public void Init(PlayerModel model)
    {
        text.text = model.Name;

        GameManager.Instance.ConnectionPreparedStram
            .Subscribe(_ =>
            {
                if (model.teamId == ClientPlayersManager.Instance.GetLocalPlayer().playerModel.teamId)
                {
                    text.color = friendTeamColor;
                }
                else
                {
                    text.color = enemyTeamColor;
                }
            });
    }
}
