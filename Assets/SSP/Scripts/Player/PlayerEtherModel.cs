using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEtherModel : EtherModel
{

    void Start()
    {
        Init();
    }

    public void AquireEther(float etheramount)
    {
        this.Ether.Value += etheramount;
    }

}
