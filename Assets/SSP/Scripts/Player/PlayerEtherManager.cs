using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerEtherManager : MonoBehaviour, IEther
{
    [SerializeField] private float initialEther;
    [SerializeField] private GameObject etherObject;
    [SerializeField] private float emitPower;

    private PlayerModel palyerModel;
    private PlayerHealthManager playerHealthManager;

    private void Start()
    {
        palyerModel = GetComponent<PlayerModel>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        palyerModel.Ether.Value = initialEther;

        playerHealthManager.GetDeathStream()
         .Where(v => v)
         .Subscribe(_ =>
         {
             EmitHalfEther();
         });
    }
    
    private void EmitHalfEther()
    {
        var halfEther = palyerModel.Ether.Value / 2.0f;
        palyerModel.Ether.Value -= halfEther;
        GenerateEtherObject(halfEther);
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

    public void SetEther(float ether)
    {
        palyerModel.Ether.Value = ether;
    }

    public float GetEther()
    {
        return palyerModel.Ether.Value;
    }

    public ReactiveProperty<float> GetEtherStream()
    {
        return palyerModel.Ether;
    }

    public void AcquireEther(float etherValue)
    {
        palyerModel.Ether.Value += etherValue;
    }

    public void ReduceEther(float ether)
    {
        palyerModel.Ether.Value -= ether;
    }
}
