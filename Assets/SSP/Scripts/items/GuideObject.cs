using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GuideObject : MonoBehaviour
{
    public string KeyCode;
    public string Description;
    public IGuideable guideable;

    private void Start()
    {
        guideable = GetComponentInParent<IGuideable>();
    }
}
