using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Test : MonoBehaviour
{

    public Subject<string> stream = new Subject<string>();

    void Start()
    {

    }
    
    void Update()
    {
        stream.OnNext("aaa");
    }
}
