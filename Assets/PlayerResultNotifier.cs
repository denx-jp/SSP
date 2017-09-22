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
            .Subscribe(winner =>
            {
                if(winner == playerModel.teamId)
                {
                    Debug.Log("勝利");
                }
                else
                {
                    Debug.Log("敗北");
                }
            });
    }
}
