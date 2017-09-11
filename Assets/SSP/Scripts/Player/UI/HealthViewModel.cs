using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class HealthViewModel : MonoBehaviour
{

    [SerializeField] private Slider sliderHealth;
    [SerializeField] private PlayerModel healthStream;

    void Start()
    {
        sliderHealth.maxValue = healthStream.GetHealth();
        healthStream.GetHealthStream().Subscribe(v => sliderHealth.value = v);
    }

}
