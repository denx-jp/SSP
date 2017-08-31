using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEtherManager : MonoBehaviour, IEther
{
    [SerializeField] private float initialEther;
    private PlayerModel palyerModel;

    private void Start()
    {
        palyerModel = GetComponent<PlayerModel>();
        palyerModel.Ether.Value = initialEther;
    }

    public void SetEther(float ether)
    {
        palyerModel.Ether.Value = ether;
    }

    public float GetEther()
    {
        return palyerModel.Ether.Value;
    }
}
