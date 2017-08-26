using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthModel : HealthModel
{

    private void Start()
    {
        Init();
    }

    public bool IsAlive()
    {
        return Health.Value > 0.0f;
    }

}
