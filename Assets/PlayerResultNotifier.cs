using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerResultNotifier : MonoBehaviour {

    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private GameJudger gameJudger;

	void Start () {

        gameJudger
            .GetWinnerStream()
            .Take(1)
            .Subscribe(winner =>
            {
                if(winner == playerModel.teamId)
                {
                    Debug.Log("Player" + playerModel.playerId +
                                "(Team" + playerModel.teamId + ")" + " :勝利");
                }
                else
                {
                    Debug.Log("Player" + playerModel.playerId +
                                "(Team" + playerModel.teamId + ")" + " :敗北");
                }
            });
    }
}
