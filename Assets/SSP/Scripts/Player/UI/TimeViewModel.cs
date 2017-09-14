using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class TimeViewModel : MonoBehaviour {

    [SerializeField] private Text textTime;
    [SerializeField] private GameObject gameManager;

    private TimeManager timeManager;

	// Use this for initialization
	void Start () {
        timeManager = gameManager.GetComponent<TimeManager>();
        //timeManager.GetCurrentTime().SubscribeToText(textTime);
	}
}
