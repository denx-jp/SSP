using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealthViewModel : MonoBehaviour {

    [SerializeField] Text uiHealthText;
    [SerializeField] GameObject player;

    private PlayerHealthManager playerHealthManager;

    void Start()
    {
        playerHealthManager = player.GetComponent<PlayerHealthManager>();
    }

    void Update()
    {
        uiHealthText.text = playerHealthManager.GetComponent<IHealth>().GetHealth().ToString();
    }
}
