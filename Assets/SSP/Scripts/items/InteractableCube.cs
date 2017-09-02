using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCube : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        this.gameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(0.0f,10.0f,0.0f));
    }
}
