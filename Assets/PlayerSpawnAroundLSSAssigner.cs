using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnAroundLSSAssigner : MonoBehaviour
{
    void SpawnAroundLSS()
    {
        var teamId = gameObject.GetComponent<PlayerModel>().teamId;
    }
}