using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Test2 : MonoBehaviour
{

    public Test test;

    void Start()
    {
        test.stream.Subscribe(s => Debug.Log(s + "OnTest2"));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
