using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRecordViewModel : MonoBehaviour
{


    [SerializeField] private Text playerNameText;
    [SerializeField] private Text killCountText;
    [SerializeField] private Text deathCountText;

    public void Init(PlayerModel playerModel, PlayerRecord record)
    {
        playerNameText.text = playerModel.playerName;
        killCountText.text = record.killCount.ToString();
        deathCountText.text = record.deathCount.ToString();
    }
}
