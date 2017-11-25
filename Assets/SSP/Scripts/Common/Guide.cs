using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Guide
{
    public string KeyCode;
    public string Description;
    [HideInInspector] public Transform transform;
}
