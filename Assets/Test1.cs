using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Test1 : MonoBehaviour
{

    public Test test;

    void Start()
    {
        test.stream.Subscribe(s => Debug.Log(s + "OnTest1"));
    }

    void Update()
    {

    }
}
