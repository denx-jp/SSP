using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnAroundLSSAssigner : MonoBehaviour
{
    public void SpawnAroundLSS(Transform _lifeSupportSystemTransform)
    {
        var teamId = gameObject.GetComponent<PlayerModel>().teamId;

        gameObject.transform.position = _lifeSupportSystemTransform.position;
    }
}