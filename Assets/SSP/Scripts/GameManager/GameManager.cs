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
    [SerializeField] private List<PlayerManager> team1Players;
    [SerializeField] private List<PlayerManager> team2Players;

    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float endDelay = 3f;

    public bool isGameStarting = false;

    private void Awake()
    {
        Instance = this;
    }

    [ServerCallback]
    private void Start()
    {
        StartCoroutine(Game());
    }

    private IEnumerator Game()
    {
        yield return StartCoroutine(GameStart());

    }

    private IEnumerator GameStart()
    {
        //初期設定
        isGameStarting = false;
        StartPanel.SetActive(true);
        BattlePanel.SetActive(false);

        //武器を生成
        //LSSをランダムな位置に移動
        //プレイヤーをLSS周辺に移動
        
        yield return new WaitForSeconds(2);

        //カウントダウン開始準備
        StartPanel.SetActive(false);
        BattlePanel.SetActive(true);

        yield return new WaitForSeconds(1);

        //カウントダウン
        for (int i = 5; i > 0; i--)
        {
            message.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        message.text = "Battle Start";
        yield return new WaitForSeconds(1);
        message.text = "";
    }
}
