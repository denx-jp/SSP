using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class toLoadScene : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }
    public void LoadBattle()
    {
        SceneManager.LoadScene("Battle");
    }

}
