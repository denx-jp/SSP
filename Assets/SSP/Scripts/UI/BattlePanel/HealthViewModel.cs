using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class HealthViewModel : MonoBehaviour
{
    [SerializeField] private Slider sliderHealth;
    [SerializeField] private GameObject playerObj;

    public IHealth healthModel;

    void Start()
    {
        if (playerObj != null)
        {
            Init(playerObj.GetComponent<IHealth>());
        }
    }

    public void Init(IHealth _healthModel)
    {
        healthModel = _healthModel;
        sliderHealth.maxValue = healthModel.GetMaxHealth();
        healthModel.GetHealthStream().Subscribe(v => sliderHealth.value = v);
    }
}