using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TittleButtonEvents : MonoBehaviour
{
    public void LoadBattle()
    {
        SceneManager.LoadScene("Battle");
    }
}
