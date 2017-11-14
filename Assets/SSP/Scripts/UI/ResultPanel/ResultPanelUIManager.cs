﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResultPanelUIManager : MonoBehaviour {

    [SerializeField] private JudgeViewModel judgeViewModel;
    [SerializeField] public List<PlayerRecordViewModel> yourTeamPlayerRecordViewModels = new List<PlayerRecordViewModel>();
    [SerializeField] public List<PlayerRecordViewModel> rivaTeamPlayerRecordViewModels = new List<PlayerRecordViewModel>();

    public void Init(bool gameResult, KillLogManager killLogManager)
    {
        judgeViewModel.Init(gameResult);

        var yourTeamID = ClientPlayersManager.Instance.GetLocalPlayer().playerModel.teamId;
        var yourTeamPlayerModels = ClientPlayersManager.Players.Select(v => v.playerModel).Where(v => v.teamId == yourTeamID).ToList();
        var rivalTeamPlayerModels = ClientPlayersManager.Players.Select(v => v.playerModel).Where(v => v.teamId != yourTeamID).ToList();

        InitTeamPlayerRecords(yourTeamPlayerModels, yourTeamPlayerRecordViewModels, killLogManager);
        InitTeamPlayerRecords(rivalTeamPlayerModels, rivaTeamPlayerRecordViewModels, killLogManager);
    }

    void InitTeamPlayerRecords(List<PlayerModel> playerModels, List<PlayerRecordViewModel> recordViewModels, KillLogManager killLog)
    {
        if (playerModels.Count == 0) return; //デバッグ時にエラーが出ないように対処
        for (int i = 0; i < playerModels.Count; i++)
        {
            var playerModel = playerModels[i];
            var playerRecord = killLog.GetPlayerRecord(playerModel.playerId);

            var recordViewModel = recordViewModels[i];
            recordViewModel.Init(playerModel,playerRecord);
            recordViewModel.gameObject.SetActive(true);
        }
    }
}
