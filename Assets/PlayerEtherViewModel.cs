using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerEtherViewModel : MonoBehaviour {

    [SerializeField] private Slider sliderEther;
    [SerializeField] private GameObject player;

    private PlayerEtherManager playerEtherManager;

	void Start()
    {
        playerEtherManager = player.GetComponent<PlayerEtherManager>();
        sliderEther.maxValue = playerEtherManager.GetComponent<IEther>().GetEther();
	}
	
	void Update()
    {
        sliderEther.value = playerEtherManager.GetComponent<IEther>().GetEther();
	}
}
