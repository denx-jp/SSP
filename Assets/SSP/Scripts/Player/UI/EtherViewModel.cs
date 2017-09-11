using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class EtherViewModel : MonoBehaviour
{

    [SerializeField] private Slider sliderEther;
    [SerializeField] private GameObject player;
    private PlayerEtherManager etherStream;

    void Start()
    {
        etherStream = player.GetComponent<PlayerEtherManager>();
        sliderEther.maxValue = etherStream.GetEther();
        etherStream.GetEtherStream().Subscribe(v => sliderEther.value = v);
    }

}
