using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class EtherViewModel : MonoBehaviour
{

    [SerializeField] private Slider sliderEther;
    [SerializeField] private IEther etherStream;

    void Start()
    {
        sliderEther.maxValue = etherStream.GetEther();
        etherStream.GetEtherStream().Subscribe(v => sliderEther.value = v);
    }

}
