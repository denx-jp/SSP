using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private ClientPlayersManager clientPlayersManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private GameJudger gameJudger;

    [SerializeField] private Text message;

    [SerializeField] private GameObject team1LSS;
    [SerializeField] private GameObject team2LSS;

    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float endDelay = 3f;

    private void Awake()
    {
        Instance = this;
    }

    [ServerCallback]
    private void Start()
    {

    }
    
    private void Game()
    {

    }
}
