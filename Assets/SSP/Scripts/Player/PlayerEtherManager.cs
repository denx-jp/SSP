using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerEtherManager : MonoBehaviour, IEtherAcquirer, IEtherEmitter
{
    [SerializeField] private GameObject etherObject;
    [SerializeField] private float emitPower;

    private PlayerModel palyerModel;
    private PlayerHealthManager playerHealthManager;

    private void Start()
    {
        palyerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();

        playerHealthManager.GetDeathStream()
             .Where(v => v)
             .Subscribe(_ =>
             {
                 var halfEther = palyerModel.Ether.Value / 2.0f;
                 EmitEther(halfEther);
                 GenerateEtherObject(halfEther);
             });
    }
    
    //非常に雑な実装なので治せるなら後から治した方がよい
    private void GenerateEtherObject(float emitEtherValue)
    {
        float emithigh = 0;
        var singleEtherValue = emitEtherValue / 10;
        while (emitEtherValue > 0)
        {
            var emittedEtherObject = Instantiate(etherObject, transform.position + Vector3.up * emithigh, transform.rotation);

            if (emitEtherValue < singleEtherValue)
                singleEtherValue = emitEtherValue;
            emitEtherValue -= singleEtherValue;
            emittedEtherObject.GetComponent<EtherObject>().Init(singleEtherValue);
            emithigh += emittedEtherObject.transform.localScale.y;      //SetEtherでemittedEtherObjectのサイズが変更されることに依存する

            var emitDirection = Vector3.up + new Vector3(Random.Range(-emitPower, emitPower), 0, Random.Range(-emitPower, emitPower));
            emittedEtherObject.GetComponent<Rigidbody>().AddForce(emitDirection, ForceMode.Impulse);

        }
    }

    public void AcquireEther(float etherValue)
    {
        palyerModel.Ether.Value += etherValue;
    }

    public void EmitEther(float ether)
    {
        palyerModel.Ether.Value -= ether;
    }
}
