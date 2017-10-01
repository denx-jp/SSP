using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable　//IInteractableを継承
{
    bool isOpen; //falseが閉まっている状態、trueが開いている状態
    public Transform Stick;
    float timer;
    //Interactメソッドが呼ばれるとisOpen切り替え、trueかfalseでOpenかCloseのメソッド呼び出し
    public void Interact(PlayerManager playerManager)
    {
        isOpen = !isOpen;
        if (isOpen)
        {   
            StartCoroutine("Open");
        }
        else if (!isOpen)
        {
            StartCoroutine("Close");
        }
    }
    void Start()
    {
        isOpen = false; //最初は閉まっている
        timer = 0;
        var player = GameObject.Find("Player");
        Stick = player.transform;
    }

    void Update()
    {
        Debug.Log(isOpen);
        //Debug.Log(Stick.position);
    }

    private IEnumerator Close()
    {
        for (int i = 0; ; i++)
        {
            timer += Time.deltaTime;
            transform.position += new Vector3(10.0f, 0.0f, 0.0f);
            Debug.Log(transform.position);

            if (timer >= 0.5f)//2秒間実行？
            {
                timer = 0;
                yield break;
            }
            yield return null;
        }

    }

    private IEnumerator Open()
    {
       for(int i=0; ; i++)
        {
            timer += Time.deltaTime;
            transform.position += new Vector3(-10.0f, 10.0f, 0.0f);
            Debug.Log(transform.position);

            if (timer >= 0.5f)
            {
                timer = 0;
                yield break;
            }
            yield return null;
        }
    }
}
