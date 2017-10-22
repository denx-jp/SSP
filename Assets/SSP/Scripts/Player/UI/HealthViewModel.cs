using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class HealthViewModel : MonoBehaviour
{
    [SerializeField] private Slider sliderHealth;
    [SerializeField] private GameObject playerObj;

    private PlayerModel playerModel;
    public IHealth healthModel;

    public void Start()
    {
        if (playerObj != null)
        {
            playerModel = playerObj.GetComponent<PlayerModel>();
            healthModel = playerModel as IHealth;

            Init();

            if (playerModel.isLocalPlayer)
                sliderHealth.gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        sliderHealth.maxValue = healthModel.GetHealth();
        healthModel.GetHealthStream().Subscribe(v => sliderHealth.value = v);
    }
}