using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealthViewModel : MonoBehaviour {

    [SerializeField] private Text uiHealthText;
    [SerializeField] private GameObject player;

    private PlayerHealthManager playerHealthManager;

    void Start()
    {
        playerHealthManager = player.GetComponent<PlayerHealthManager>();
    }

    void Update()
    {
        // デバッグ用
        uiHealthText.text = playerHealthManager.GetComponent<IHealth>().GetHealth().ToString();
    }
}
