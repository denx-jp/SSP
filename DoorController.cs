using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable　//IInteractableを継承
{
    bool isOpen; //falseが閉まっている状態、trueが開いている状態
    //Interactメソッドが呼ばれるとisOpen切り替え、trueかfalseでOpenかCloseのメソッド呼び出し
    public void Interact(PlayerManager playerManager)
    {
        isOpen = !isOpen;
    }
    void Start()
    {
        isOpen = false; //最初は閉まっている
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
    }

    private void Close()
    {
        iTween.MoveTo(gameObject, new Vector3(10, 0, 0), 2.0f);
    }

    private void Open()
    {
        iTween.MoveTo(gameObject, new Vector3(-10, 0, 0), 2.0f);        
    }
}
