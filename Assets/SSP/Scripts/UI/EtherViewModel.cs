using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class EtherViewModel : MonoBehaviour
{

    [SerializeField] private Slider sliderEther;
    public IEther etherModel;

    public void Init()
    {
        sliderEther.maxValue = etherModel.GetEther();
        etherModel.GetEtherStream().Subscribe(v => sliderEther.value = v);
    }

}
