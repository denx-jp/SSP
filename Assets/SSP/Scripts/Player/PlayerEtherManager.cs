using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Triggers;

public class PlayerEtherManager : NetworkBehaviour, IEtherAcquirer, IEtherEmitter
{
    [SerializeField] private GameObject etherObject;
    [SerializeField] private float emitPower;
    [SerializeField] private Vector3 emitDirectionRange;
    [SerializeField] private GameObject etherDetector;

    private PlayerModel playerModel;
    private PlayerHealthManager playerHealthManager;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();

        playerHealthManager.GetDeathStream()
             .Where(v => v)
             .Where(_ => isLocalPlayer)
             .Where(_ => playerModel.syncEther > 0)
             .Subscribe(_ =>
             {
                 var halfEther = playerModel.syncEther / 2.0f;
                 EmitEther(halfEther);
                 CmdGenerateEtherObject(halfEther);
             });

        // エーテルオブジェクト検出
        etherDetector.OnTriggerEnterAsObservable()
            .Select(v => v.GetComponent<EtherObject>())
            .Where(v => v != null && v.target == null)
            .Subscribe(etherObject =>
            {
                etherObject.CmdSetTarget(gameObject);
            });
    }

    [Command]
    private void CmdGenerateEtherObject(float emitEtherValue)
    {
        float emithigh = transform.localScale.y / 2;
        var singleEtherValue = emitEtherValue / 10;
        while (emitEtherValue > 0)
        {
            var emittedEtherObject = Instantiate(etherObject, transform.position + Vector3.up * emithigh, transform.rotation);

            if (emitEtherValue < singleEtherValue) singleEtherValue = emitEtherValue;
            emitEtherValue -= singleEtherValue;
            emittedEtherObject.GetComponent<EtherObject>().Init(singleEtherValue);

            emithigh += emittedEtherObject.transform.localScale.y;
            var emitDirection = new Vector3(
                Random.Range(-emitDirectionRange.x, emitDirectionRange.x),
                Random.Range(emitDirectionRange.y / 2, emitDirectionRange.y),
                Random.Range(-emitDirectionRange.z, emitDirectionRange.z)).normalized;
            emittedEtherObject.GetComponent<Rigidbody>().velocity = emitDirection * emitPower;
            NetworkServer.SpawnWithClientAuthority(emittedEtherObject, connectionToClient);
        }
    }

    public void AcquireEther(float etherValue)
    {
        playerModel.syncEther += etherValue;
    }

    public void EmitEther(float etherValue)
    {
        playerModel.syncEther -= etherValue;
    }
}
