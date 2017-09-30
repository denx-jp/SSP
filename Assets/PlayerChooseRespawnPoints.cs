using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerChooseRespawnPoints : MonoBehaviour {

    private PlayerInputManager pim;
    [SerializeField] private List<GameObject> RespawnPoints = new List<GameObject>();
    void Start () {

        pim = GetComponent<PlayerInputManager>();
        pim.ChooseRespawnPointsButtonDown
            .Where(v => v)
            .Where(v => Input.GetButtonDown("Right"))
            .Subscribe(v =>
            {
                
            });
        pim.ChooseRespawnPointsButtonDown
            .Where(v => v)
            .Where(v => Input.GetButtonDown("Left"))
            .Subscribe(v =>
            {

            });
    }
}
