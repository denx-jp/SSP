using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class PlayerEtherManager : NetworkBehaviour, IEtherAcquirer, IEtherEmitter
{
    [SerializeField] private GameObject etherObject;
    [SerializeField] private float emitPower;

    private PlayerModel playerModel;
    private PlayerHealthManager playerHealthManager;

    private struct PlayerEtherProp{
        public float value;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 direction;

        public PlayerEtherProp(float _value, Vector3 _position, Quaternion _rotation, Vector3 _direction)
        {
            value = _value; position = _position;
            rotation = _rotation; direction = _direction;
        }
    }
    private class PlayerEtherProps : SyncListStruct<PlayerEtherProp>{ };
    private PlayerEtherProps playerEtherProps;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        playerEtherProps = new PlayerEtherProps();

        playerHealthManager.GetDeathStream()
             .Where(v => v)
             .Subscribe(_ =>
             {
                 CmdStartPlayerEtherPop();
             });
    }

#if ONLINE
    [Command]
#endif
    private void CmdStartPlayerEtherPop()
    {
        var halfEther = playerModel.Ether.Value / 2.0f;
        EmitEther(halfEther);
        GenerateEtherObjectProp(halfEther);

        RpcStartPlayerEtherPop();
    }
#if ONLINE
    [ClientRpc]
#endif
    private void RpcStartPlayerEtherPop()
    {
        float emithigh = 0;
        foreach(var etherProp in playerEtherProps)
        {
            var emittedEtherObject = Instantiate(etherObject, etherProp.position + Vector3.up * emithigh, etherProp.rotation);
            emittedEtherObject.GetComponent<EtherObject>().Init(etherProp.value);
            emithigh += emittedEtherObject.transform.localScale.y;
            emittedEtherObject.GetComponent<Rigidbody>().AddForce(etherProp.direction, ForceMode.Impulse);
        }
    }

    //非常に雑な実装なので治せるなら後から治した方がよい
    private void GenerateEtherObjectProp(float emitEtherValue)
    {
        float singleEtherValue = emitEtherValue / 10;
        while (emitEtherValue > 0)
        {
            if (emitEtherValue < singleEtherValue)
                singleEtherValue = emitEtherValue;
            emitEtherValue -= singleEtherValue;

            playerEtherProps.Add(new PlayerEtherProp(
                    singleEtherValue,
                    transform.position,
                    transform.rotation,
                    (Vector3.up + new Vector3(Random.Range(-emitPower, emitPower), 0, Random.Range(-emitPower, emitPower))))
                );
        }
    }

    public void AcquireEther(float etherValue)
    {
        playerModel.Ether.Value += etherValue;
    }

    public void EmitEther(float ether)
    {
        playerModel.Ether.Value -= ether;
    }
}
