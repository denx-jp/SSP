using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public Subject<Unit> ConnectionPreparedStram = new Subject<Unit>();

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

        this.UpdateAsObservable()
            .Where(_ => IsGameStarting())
            .Subscribe(_ =>
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            });
    }

    private IEnumerator GameStart()
    {

        yield return new WaitForSeconds(startDelay);

        // すべての準備が整ったことを確認するのを待つ
        while (ClientPlayersManager.Players.Count < NetworkServer.connections.Count)
        {
            yield return null;
        }

        #region ID割り当て        
        var players = ClientPlayersManager.Players;
        var playerCount = players.Count;
        players.Select((player, index) => new { index, player }).ToList().ForEach(v => v.player.playerModel.Id = v.index);
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
        weaponPopper.Init();
        spawnPointManager.Init(team1LSS.transform, team2LSS.transform);

        // LSSをランダムな位置に移動
        team1LSS.transform.position = SpawnPointManager.Instance.GetRandomSpawnPosition();
        while (true)
        {
            var spawnPos = SpawnPointManager.Instance.GetRandomSpawnPosition();
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
            var pos = SpawnPointManager.Instance.GetSpawnPositionAroundLSS(player.playerModel.teamId);
            RpcMovePlayer(player.gameObject, pos);
        }

        //カウントダウン開始準備
        RpcShowBattleScene();

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
    void RpcMovePlayer(GameObject player, Vector3 pos)
    {
        player.transform.position = pos;
    }

    [ClientRpc]
    void RpcPrepareGame()
    {
        killLogManager.Init();
        gameJudger.Init(team1LSS.GetComponent<LifeSupportSystemEtherManager>(), team2LSS.GetComponent<LifeSupportSystemEtherManager>());
        isGameStarting = false;
        var battleUI = BattlePanel.GetComponent<PlayerBattleUIManager>();
        var player = clientPlayersManager.GetLocalPlayer();
        var friendLss = player.playerModel.teamId == 1 ? team1LSS : team2LSS;
        var enemyLss = player.playerModel.teamId == 1 ? team2LSS : team1LSS;
        battleUI.Init(player, friendLss.GetComponent<LifeSupportSystemModel>(), enemyLss.GetComponent<LifeSupportSystemModel>());
        StartPanel.SetActive(true);
        BattlePanel.SetActive(false);
        message.text = string.Empty;

        ConnectionPreparedStram.OnNext(Unit.Default);
    }

    [ClientRpc]
    void RpcShowBattleScene()
    {
        clientPlayersManager.GetLocalPlayer().playerCameraController.LookPlayer();
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ResultPanel.SetActive(true);

        yield return new WaitForSeconds(endDelay);

        message.text = isWin ? "Victory" : "Defeat";

        yield return new WaitForSeconds(endDelay);

        isGameStarting = false;
        message.text = string.Empty;
        var result = ResultPanel.transform.Find("Result").gameObject;
        result.SetActive(true);
        result.GetComponent<ResultPanelUIManager>().Init(isWin, killLogManager);

        yield return new WaitForSeconds(10);

        // 次のゲームのためにstatic初期化
        ClientPlayersManager.Players = new List<PlayerManager>();

        SceneManager.LoadScene(TitleScene);
    }
}