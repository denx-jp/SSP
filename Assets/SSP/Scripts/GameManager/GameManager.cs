using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private ClientPlayersManager clientPlayersManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private GameJudger gameJudger;

    [SerializeField] private Text message;
    [SerializeField] private GameObject StartPanel;
    [SerializeField] private GameObject BattlePanel;

    [SerializeField] private LifeSupportSystemEtherManager team1LSS;
    [SerializeField] private LifeSupportSystemEtherManager team2LSS;

    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float endDelay = 3f;

    [SyncVar] public bool isGameStarting = false;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Instance = this;
    }

    [ServerCallback]
    private void Start()
    {
        StartCoroutine(GameStart());
    }

    private IEnumerator GameStart()
    {
        //初期設定
        RpcPrepareGame();

        //武器を生成
        //LSSをランダムな位置に移動
        //プレイヤーをLSS周辺に移動

        yield return new WaitForSeconds(5);

        //カウントダウン開始準備
        StartPanel.SetActive(false);
        BattlePanel.SetActive(true);

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

    [ClientRpc]
    void RpcPrepareGame()
    {
        isGameStarting = false;
        var battleUI = BattlePanel.GetComponent<PlayerBattleUIManager>();
        battleUI.Init(clientPlayersManager.GetLocalPlayer(), clientPlayersManager, timeManager);
        StartPanel.SetActive(true);
        BattlePanel.SetActive(false);
        message.text = string.Empty;
    }

    [ClientRpc]
    void RpcBattleStart()
    {
        isGameStarting = true;
        team1LSS.Init();
        team2LSS.Init();
        message.text = "Battle Start";
    }

    [ClientRpc]
    void RpcChangeMessage(string msg)
    {
        message.text = msg;
    }
}
