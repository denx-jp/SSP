using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerModel : NetworkBehaviour, IHealth, IEther
{
    [SyncVar] public int playerId = 0;
    [SyncVar] public int teamId = 0;
    [SyncVar] public float syncHealth;
    [SyncVar] public float syncEther;
    public ReactiveProperty<float> Health { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> Ether { get; private set; } = new ReactiveProperty<float>();
    [SerializeField] private float initialHealth;
    [SerializeField] private float initialEther;

    [SerializeField] public bool isLocalPlayerCharacter = false;
    //ネットワーク実装時にはローカルプレイヤーのみLayerMap.LocalPlayerになる。
    [HideInInspector] public int defaultLayer = LayerMap.Default;
    [SyncVar] int moveModeIndex;
    [SyncVar] public string playerName;
    public MoveMode MoveMode
    {
        get { return (MoveMode)moveModeIndex; }
        set
        {
            if (isLocalPlayer)
                CmdSetMovemode((int)value);
            else
                moveModeIndex = (int)value;
        }
    }

    private void Awake()
    {
        syncEther = initialEther;
        Ether.Value = syncEther;
        MoveMode = MoveMode.normal;

        this.ObserveEveryValueChanged(_ => syncHealth).Subscribe(v => Health.Value = v);
        this.ObserveEveryValueChanged(_ => syncEther).Subscribe(v => Ether.Value = v);

        Init();
    }

    public void Init()
    {
        syncHealth = initialHealth;
        Health.Value = syncHealth;
    }

    #region Health
    public float GetMaxHealth()
    {
        return initialHealth;
    }

    public ReactiveProperty<float> GetHealthStream()
    {
        return Health;
    }

    public bool IsAlive()
    {
        return Health.Value > 0.0f;
    }
    #endregion

    #region Ether
    public float GetEther()
    {
        return Ether.Value;
    }

    public float GetMaxEther()
    {
        return initialEther;
    }

    public ReactiveProperty<float> GetEtherStream()
    {
        return Ether;
    }
    #endregion

    [Command]
    private void CmdSetMovemode(int index)
    {
        moveModeIndex = index;
    }
}
