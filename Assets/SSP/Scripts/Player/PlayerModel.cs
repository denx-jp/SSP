using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerModel : NetworkBehaviour, IHealth, IEther
{
    [SyncVar] public int playerId = 0;
    [SyncVar] public int teamId = 0;
    [SyncVar(hook = "SyncHealth")] public float syncHealth;
    [SyncVar(hook = "SyncEther")] public float syncEther;
    public ReactiveProperty<float> Health { get; private set; }
    public ReactiveProperty<float> Ether { get; private set; }
    [SerializeField] private float initialHealth;
    [SerializeField] private float initialEther;

    [SerializeField] public bool isLocalPlayerCharacter = false;
    //ネットワーク実装時にはローカルプレイヤーのみLayerMap.LocalPlayerになる。
    [HideInInspector] public int defaultLayer = LayerMap.Default;   

    private void Awake()
    {
        Health = new ReactiveProperty<float>();
        Ether = new ReactiveProperty<float>();
        if (playerId == 0) playerId = Random.Range(1, 100);
        if (teamId == 0) teamId = Random.Range(1, 100);
        syncEther = initialEther;

        Init();
    }

    public void Init()
    {
        syncHealth = initialHealth;
    }

    public float GetHealth()
    {
        return Health.Value;
    }

    public ReactiveProperty<float> GetHealthStream()
    {
        return Health;
    }

    public bool IsAlive()
    {
        return Health.Value > 0.0f;
    }

    public float GetEther()
    {
        return Ether.Value;
    }

    public ReactiveProperty<float> GetEtherStream()
    {
        return Ether;
    }

    [ClientCallback]
    private void SyncHealth(float value)
    {
        syncHealth = value;
        Health.Value = value;
    }

    [ClientCallback]
    private void SyncEther(float value)
    {
        syncEther = value;
        Ether.Value = value;
    }
}
