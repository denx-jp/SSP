using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class SwitchScope : MonoBehaviour
{

    [SerializeField] private GameObject scopeCam;
    [SerializeField] private GameObject cam;
    PlayerInputManager pIM;

    void Start()
    {
        pIM = GetComponent<PlayerInputManager>();
        pIM.ScopeButtonDown
           .Where(v => v)
           .Subscribe(_ =>
           {
            if (scopeCam.activeInHierarchy)
               {
                   scopeCam.SetActive(false);
                   cam.SetActive(true);
               }
               else
               {
                   cam.SetActive(false);
                   scopeCam.SetActive(true);
               }
           });
    }
}
