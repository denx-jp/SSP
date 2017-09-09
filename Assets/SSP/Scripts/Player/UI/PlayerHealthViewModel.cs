using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class PlayerHealthViewModel : MonoBehaviour
{

    [SerializeField] private Slider sliderHealth;
    [SerializeField] private IHealth healthStream;

    void Start()
    {
        sliderHealth.maxValue = healthStream.GetHealth();
        healthStream.GetHealthStream().Subscribe(v => sliderHealth.value = v);
    }

}
