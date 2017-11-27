using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TittleButtonEvents : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GameStartButtonEvent()
    {
        SceneManager.LoadScene(SceneMap.Lobby);
    }

    public void QuitGameButtonEvent()
    {
        Application.Quit();
    }
}
