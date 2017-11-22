using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class EtherViewModel : MonoBehaviour
{

    [SerializeField] private Slider slider;
    public IEther etherModel;

    public void Init(IEther _etherModel)
    {
        etherModel = _etherModel;
        slider.maxValue = etherModel.GetMaxEther();
        etherModel.GetEtherStream().Subscribe(v => slider.value = v);
    }

}
