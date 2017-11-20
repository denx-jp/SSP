using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UniRx;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private ClientPlayersManager clientPlayersManager;
    [SerializeField] private GameJudger gameJudger;
    [SerializeField] private KillLogManager killLogManager;
    [SerializeField] private EtherPopper etherPopper;
    [SerializeField] private WeaponPopper weaponPopper;
    [SerializeField] private SpawnPointManager spawnPointManager;

    [SerializeField] private Text message;
    [SerializeField] private GameObject StartPanel;
    [SerializeField] private GameObject BattlePanel;
    [SerializeField] private GameObject ResultPanel;

    [SerializeField] private GameObject team1LSS;
    [SerializeField] private GameObject team2LSS;
    [SerializeField] private float minLssDistance;

    [SerializeField] private float startDelay = 3f;
    [SerializeField] private int countDownCount = 5;
    [SerializeField] private float endDelay = 3f;
    [SerializeField] private string TitleScene;
    [SyncVar] private bool isGameStarting = false;

    public static bool IsGameStarting()
    {
        if (Instance == null) return false;
        return Instance.isGameStarting;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(GameStart());
        }

        gameJudger.GetJudgeStream()
            .Take(1)
            .Subscribe(v =>
            {
                StartCoroutine(GameEnd(v));
            });
    }

    private IEnumerator GameStart()
    {
        // すべての準備が整ったことを確認するのを待つ
        while (ClientPlayersManager.Players.Count < NetworkServer.connections.Count)
        {
            yield return null;
        }

        #region ID割り当て        
        var players = ClientPlayersManager.Players;
        var playerCount = players.Count;
        players.Select((player, index) => new { index, player }).ToList().ForEach(v => v.player.playerModel.playerId = v.index + 1);
        players.OrderBy(i => Guid.NewGuid()).Select((player, index) => new { index, player }).ToList().ForEach(v =>
        {
            if (v.index < playerCount / 2.0)
                v.player.playerModel.teamId = 1;
            else
                v.player.playerModel.teamId = 2;
        });
        #endregion

        // IDの同期を待つ
        yield return new WaitForSeconds(1);

        RpcPrepareGame();
        gameJudger.Init(team1LSS.GetComponent<LifeSupportSystemEtherManager>(), team2LSS.GetComponent<LifeSupportSystemEtherManager>());
        weaponPopper.Init();
        spawnPointManager.Init(team1LSS.transform, team2LSS.transform);

        // LSSをランダムな位置に移動
        team1LSS.transform.position = SpawnPointManager.Instance.GetRandomSpawnPoint().position;
        while (true)
        {
            var spawnPos = SpawnPointManager.Instance.GetRandomSpawnPoint().position;
            var distance = Vector3.Distance(team1LSS.transform.position, spawnPos);

            if (distance > minLssDistance)
            {
                team2LSS.transform.position = spawnPos;
                break;
            }
        }

        //プレイヤーをLSS周辺に移動
        foreach (var player in ClientPlayersManager.Players)
        {
            player.transform.position = SpawnPointManager.Instance.GetSpawnPointAroundLSS(player.playerModel.teamId).position;
        }
        clientPlayersManager.GetLocalPlayer().playerCameraController.LookPlayer();

        yield return new WaitForSeconds(startDelay);

        //カウントダウン開始準備
        RpcSwapPanel();

        yield return new WaitForSeconds(1);

        //カウントダウン
        for (int i = countDownCount; i > 0; i--)
        {
            RpcChangeMessage(i.ToString());
            yield return new WaitForSeconds(1);
        }

        //戦闘開始
        RpcBattleStart();
        yield return new WaitForSeconds(1);
        RpcChangeMessage(string.Empty);
    }

    #region Startまわりメソッド
    [ClientRpc]
    void RpcPrepareGame()
    {
        killLogManager.Init();
        isGameStarting = false;
        var battleUI = BattlePanel.GetComponent<PlayerBattleUIManager>();
        var player = clientPlayersManager.GetLocalPlayer();
        var lss = player.playerModel.teamId == 1 ? team1LSS : team2LSS;
        battleUI.Init(player, lss.GetComponent<LifeSupportSystemModel>());
        StartPanel.SetActive(true);
        BattlePanel.SetActive(false);
        message.text = string.Empty;
    }

    [ClientRpc]
    void RpcSwapPanel()
    {
        StartPanel.SetActive(false);
        BattlePanel.SetActive(true);
    }

    [ClientRpc]
    void RpcBattleStart()
    {
        isGameStarting = true;
        etherPopper.Init();
        team1LSS.GetComponent<LifeSupportSystemEtherManager>().Init();
        team2LSS.GetComponent<LifeSupportSystemEtherManager>().Init();
        message.text = "Battle Start";
    }

    [ClientRpc]
    void RpcChangeMessage(string msg)
    {
        message.text = msg;
    }
    #endregion

    private IEnumerator GameEnd(bool isWin)
    {
        ResultPanel.SetActive(true);

        yield return new WaitForSeconds(endDelay);

        message.text = isWin ? "Victory" : "Defeat";

        yield return new WaitForSeconds(endDelay);

        isGameStarting = false;
        message.text = string.Empty;
        var result = ResultPanel.transform.Find("Result").gameObject;
        result.SetActive(true);
        result.GetComponent<ResultPanelUIManager>().Init(isWin, killLogManager);

        yield return new WaitForSeconds(30);

        SceneManager.LoadScene(TitleScene);
    }
}