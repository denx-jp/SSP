using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerEtherViewModel : MonoBehaviour {

    [SerializeField] private Text uiEtherText;
    [SerializeField] private GameObject player;

    private PlayerEtherManager playerEtherManager;

	// Use this for initialization
	void Start () {
        playerEtherManager = player.GetComponent<PlayerEtherManager>();
	}
	
	// Update is called once per frame
	void Update () {
        // デバッグ用
        uiEtherText.text = playerEtherManager.GetComponent<IEther>().GetEther().ToString();
	}
}
