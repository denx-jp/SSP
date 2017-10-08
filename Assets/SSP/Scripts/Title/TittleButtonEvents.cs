using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TittleButtonEvents : MonoBehaviour
{
    [SerializeField] private string MatchingSceneName;

    public void GameStartButtonEvent()
    {
        SceneManager.LoadScene(MatchingSceneName);
    }

    public void QuitGameButtonEvent()
    {
        Application.Quit();
    }
}
