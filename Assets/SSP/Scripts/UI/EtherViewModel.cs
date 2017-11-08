using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class EtherViewModel : MonoBehaviour
{

    [SerializeField] private Text text;
    public IEther etherModel;

    public void Init()
    {
        etherModel.GetEtherStream().Subscribe(v => text.text = $"Ether : {v.ToString()}");
    }

}
