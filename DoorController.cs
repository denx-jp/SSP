using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable　//IInteractableを継承
{
    bool isOpen; //falseが閉まっている状態、trueが開いている状態
    float maxDistance;
    float minDistance;
    public Transform Stick;
    float timer;
    //Interactメソッドが呼ばれるとisOpen切り替え
    public void Interact(PlayerManager playerManager)
    {
        isOpen = !isOpen;
       
    }
    void Start()
    {
        isOpen = false; //最初は閉まっている
        timer = 0;
        maxDistance = 20.0f;
        minDistance = 0;
        var player = GameObject.Find("Player");
        Stick = player.transform;
        var tp = transform.position;
        //var RegularedDistance = Mathf.Clamp(tp, minDistance, maxDistance);まだです
        //transform.position = new Vector3(RegularedDistance, 0, 0);まだです
    }

    void Update()
    {
        if (isOpen)
        {
            Open();
        }
        else if (!isOpen)
        {
            Close();
        }
        Debug.Log(isOpen);
        //Debug.Log(Stick.position);
    }

    private void Close()
    {
        for (int i = 0; ; i++)
        {
            timer += Time.deltaTime;
            transform.position += new Vector3(10.0f, 0.0f, 0.0f);
            Debug.Log(transform.position);

           
        }

    }

    private void Open()
    {
       for(int i=0; ; i++)
        {
            timer += Time.deltaTime;
            transform.position += new Vector3(-10.0f, 10.0f, 0.0f);
            Debug.Log(transform.position);

            
        }
    }
}
