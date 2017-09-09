using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealthViewModel : MonoBehaviour {

    [SerializeField] private Slider sliderHealth;
    [SerializeField] private GameObject player;

    private PlayerHealthManager playerHealthManager;

    void Start()
    {
        playerHealthManager = player.GetComponent<PlayerHealthManager>();
        sliderHealth.maxValue = playerHealthManager.GetComponent<IHealth>().GetHealth();
    }

    void Update()
    {
        sliderHealth.value = playerHealthManager.GetComponent<IHealth>().GetHealth();
    }
}
