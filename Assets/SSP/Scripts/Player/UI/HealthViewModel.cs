using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class HealthViewModel : NetworkBehaviour
{

    [SerializeField] private Slider sliderHealth;
    [SerializeField] private GameObject playerObj;

    private PlayerModel playerModel;
    private GameObject playerCamera;

    public IHealth healthModel;

    public void Start()
    {
        if (playerObj != null)
        {
            playerModel = GetComponent<PlayerModel>();
            healthModel = playerModel as IHealth;

            playerCamera = GameObject.FindWithTag("MainCamera");

            Init();

            // 他プレイヤーのHPバーを自HUDに対して垂直に表示
            if (playerCamera != null)
                this.ObserveEveryValueChanged(_ => playerCamera.transform.rotation)
                    .Subscribe(v => sliderHealth.transform.rotation = v);

            if (isLocalPlayer)
                sliderHealth.gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        sliderHealth.maxValue = healthModel.GetHealth();
        healthModel.GetHealthStream().Subscribe(v => sliderHealth.value = v);
    }
}
