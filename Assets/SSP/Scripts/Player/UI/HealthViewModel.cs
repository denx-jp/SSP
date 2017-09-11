using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class HealthViewModel : MonoBehaviour
{

    [SerializeField] private Slider sliderHealth;
    [SerializeField] private GameObject player;
    private PlayerModel healthStream;

    void Start()
    {
        healthStream = player.GetComponent<PlayerModel>();
        sliderHealth.maxValue = healthStream.GetHealth();
        healthStream.GetHealthStream().Subscribe(v => sliderHealth.value = v);
    }

}
