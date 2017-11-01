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

    [SerializeField] private Text message;
    [SerializeField] private GameObject StartPanel;
    [SerializeField] private GameObject BattlePanel;
    [SerializeField] private GameObject ResultPanel;

    [SerializeField] private GameObject team1LSS;
    [SerializeField] private GameObject team2LSS;

    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float endDelay = 3f;
    [SerializeField] private string TitleScene;
    [SyncVar] public bool isGameStarting = false;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Instance = this;
    }

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(GameStart());
        }

        gameJudger.GetJudgeStream()
            .Subscribe(v =>
            {
                StartCoroutine(GameEnd(v));
            });
    }

    private IEnumerator GameStart()
    {
        // すべての準備が整ったことを確認するのを待つ
        yield return new WaitForSeconds(1);

        RpcPrepareGame();
        //初期設定
        //武器を生成 
        //プレイヤーをLSS周辺に移動

        yield return new WaitForSeconds(startDelay);

        //カウントダウン開始準備
        RpcSwapPanel();

        yield return new WaitForSeconds(1);

        //カウントダウン
        for (int i = 5; i > 0; i--)
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
        isGameStarting = false;
        var battleUI = BattlePanel.GetComponent<PlayerBattleUIManager>();
        var player = clientPlayersManager.GetLocalPlayer();
        var lss = player.playerModel.teamId == 1 ? team1LSS : team2LSS;
        battleUI.Init(player, lss.GetComponent<LifeSupportSystemModel>(), clientPlayersManager);
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
        ResultPanel.transform.Find("Result").gameObject.SetActive(true);

        yield return new WaitForSeconds(30);

        SceneManager.LoadScene(TitleScene);
    }
}
