﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class HealthViewModel : MonoBehaviour
{
    [SerializeField] private Slider sliderHealth;
    [SerializeField] private GameObject playerObj;

    public IHealth healthModel;

    public void Start()
    {
        if (playerObj != null)
        {
            healthModel = playerObj.GetComponent<PlayerModel>() as IHealth;
            Init();
        }
    }

    public void Init()
    {
        sliderHealth.maxValue = healthModel.GetHealth();
        healthModel.GetHealthStream().Subscribe(v => sliderHealth.value = v);
    }
}