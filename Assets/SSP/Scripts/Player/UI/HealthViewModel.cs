using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class HealthViewModel : MonoBehaviour
{

    [SerializeField] private Slider sliderHealth;
    public IHealth healthModel;

    public void Init()
    {
        sliderHealth.maxValue = healthModel.GetHealth();
        healthModel.GetHealthStream().Subscribe(v => sliderHealth.value = v);
    }

}
